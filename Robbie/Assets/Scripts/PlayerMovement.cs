using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    private Rigidbody2D rb;
    private BoxCollider2D bColl;

    public float xVelocity;

    [Header("移动参数")]
    public float speed = 8f;
    public float crouchSpeedDivisor = 3f; // 下蹲时的移动速度

    [Header("状态")]
    public bool isCrouch; // 是否处于下蹲状态
    public bool isOnGround; // 是否正在地上
    public bool isJump; // 是否处于跳跃状态
    public bool isHeadBlocked; // 头顶是否处于封死状态
    public bool isHanging; // 是否处于悬挂

    [Header("环境检测")]
    public LayerMask groundLayer; // 地面
    public float footOffset = 0.4f; // 左右两个脚的距离，根据 角色碰撞体宽/2 求得
    public float headClearance = 0.5f; // 头顶检测距离
    public float groundDistance = 0.2f; // 检测与地面之间的距离
    // 悬挂
    private float playerHeight; // 角色高度
    public float eyeHeight = 1.5f; // 角色眼睛高度的射线
    public float grapDistance = 0.4f; // 游戏角色距离前面的墙有多远
    public float reachOffset = 0.7f; // 头顶往上没有墙壁，头顶往下有墙壁，而且，他距离我的游戏角色有一小段的距离


    [Header("跳跃参数")]
    public float jumpForce = 6.3f; // 跳跃加成
    public float jumpHoldForce = 1.9f; // 长按跳跃加成
    public float jumpHoldDuration = 0.1f; // 跳跃按键可以按多长时间
    public float crouchJumpBoost = 2.5f; // 下蹲跳跃加成
    private float jumpTime; // 跳跃时间，和 jumpHoldDuration 配合使用
    // 悬挂
    public float hangingJumpForce = 15f; // 悬挂后的跳跃


    // 碰撞体的尺寸
    private Vector2 collStandSize;
    private Vector2 collStandOffset;
    private Vector2 collCrouchSize;
    private Vector2 collCrouchOffset;

    // 按键信息
    private bool jumpPressed; // 单次按下跳跃
    private bool jumpHold; // 长按跳跃
    private bool crouchHold; // 长按下蹲
    private bool crouchPressed; // 单次下蹲，用于悬挂的下落

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        bColl = GetComponent<BoxCollider2D>();
        // 角色高度
        playerHeight = bColl.size.y;
        // 初始站立时的，碰撞体信息
        collStandSize = bColl.size;
        collStandOffset = bColl.offset;
        // 下蹲时的碰撞体信息，碰撞体应该变为原来的一半
        collCrouchSize = new Vector2(collStandSize.x, collStandSize.y / 2f);
        collCrouchOffset = new Vector2(collStandOffset.x, collStandOffset.y / 2f);
    }

    void Update() {
        // 按键检测
        jumpPressed = Input.GetButtonDown("Jump");
        jumpHold = Input.GetButton("Jump");
        crouchHold = Input.GetButton("Crouch");
        crouchPressed = Input.GetButtonDown("Crouch");
    }

    private void FixedUpdate() {
        GroundMovement();
        PhysicsCheck();
        MidAirMovement();
    }

    /// <summary>
    /// 物理环境判断，射线检测
    /// </summary>
    private void PhysicsCheck() {
        // 是否在地面上
        RaycastHit2D leftCheck = Raycast(new Vector2(-footOffset, 0f), Vector2.down, groundDistance, groundLayer); // 左脚与地面的射线检测
        RaycastHit2D rightCheck = Raycast(new Vector2(footOffset, 0f), Vector2.down, groundDistance, groundLayer); // 右脚与地面的射线检测
        if (leftCheck || rightCheck) {
            isOnGround = true;
        }
        else {
            isOnGround = false;
        }

        // 是否头顶是否被挡住了
        RaycastHit2D headCheck = Raycast(new Vector2(0f, bColl.size.y), Vector2.up, headClearance, groundLayer); // 头顶射线检测
        isHeadBlocked = headCheck;

        // 悬挂
        float direction = transform.localScale.x; // 角色的方向，左：-1，右：1
        Vector2 grapDir = new Vector2(direction, 0f); // 射线的方向
        RaycastHit2D blockedCheck = Raycast(new Vector2(footOffset * direction, playerHeight), grapDir, grapDistance, groundLayer); // 头顶水平射线
        RaycastHit2D wallCheck = Raycast(new Vector2(footOffset * direction, eyeHeight), grapDir, grapDistance, groundLayer); // 眼睛水平射线
        RaycastHit2D ledgeCheck = Raycast(new Vector2(reachOffset * direction, playerHeight), Vector2.down, grapDistance, groundLayer); // 竖直射线
        if (!isOnGround && rb.velocity.y < 0f && ledgeCheck && wallCheck && !blockedCheck) { // 悬挂条件
            // 角色位置的修正
            Vector3 pos = transform.position; //角色当前的位置
            pos.x += (wallCheck.distance - 0.05f) * direction; // wallCheck.distance：我的角色离墙面的距离,0.05f：贴图手臂的位置
            pos.y -= ledgeCheck.distance; // 减去竖直射线高出的部分
            transform.position = pos;
            // 角色静止，实现悬挂
            rb.bodyType = RigidbodyType2D.Static;
            isHanging = true;
        }
    }

    /// <summary>
    /// 射线检测
    /// </summary>
    /// <param name="offset">距角色的位置偏移</param>
    /// <param name="rayDiraction">射线方向</param>
    /// <param name="length">射线长度</param>
    /// <param name="layer">需要检测的层</param>
    /// <returns></returns>
    private RaycastHit2D Raycast(Vector2 offset, Vector2 rayDiraction, float length, LayerMask layer) {
        Vector2 pos = transform.position; // 角色位置
        RaycastHit2D hit = Physics2D.Raycast(pos + offset, rayDiraction, length, layer); // 射线检测
        Color color = hit ? Color.red : Color.green; // 碰撞到了就红色，没有碰撞就绿色
        Debug.DrawRay(pos + offset, rayDiraction * length, color); // 画射线
        return hit;
    }

    /// <summary>
    /// 角色移动
    /// </summary>
    private void GroundMovement() {
        if (isHanging) { // 悬挂后禁止移动
            return;
        }
        if (crouchHold && !isCrouch && isOnGround) { // 下蹲
            Crouch();
        }
        else if ((!crouchHold && isCrouch && !isHeadBlocked) || (!isOnGround && isCrouch)) { // 站立
            StandUp();
        }
        xVelocity = Input.GetAxis("Horizontal");
        if (isCrouch) { // 如果处于下蹲状态，减慢移动速度
            xVelocity /= crouchSpeedDivisor;
        }
        rb.velocity = new Vector2(xVelocity * speed, rb.velocity.y);
        FilpDirction();
    }

    /// <summary>
    /// 角色朝向
    /// </summary>
    private void FilpDirction() {
        if (xVelocity < 0) {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (xVelocity > 0) {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    /// <summary>
    /// 下蹲
    /// </summary>
    private void Crouch() {
        isCrouch = true;
        // 下蹲时的碰撞体信息
        bColl.size = collCrouchSize;
        bColl.offset = collCrouchOffset;
    }

    /// <summary>
    /// 站立
    /// </summary>
    private void StandUp() {
        isCrouch = false;
        // 站立时的碰撞体信息
        bColl.size = collStandSize;
        bColl.offset = collStandOffset;
    }

    /// <summary>
    /// 跳跃有关的功能
    /// </summary>
    private void MidAirMovement() {
        // 悬挂
        if (isHanging) {
            if (jumpPressed) { // 悬挂跳跃
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.velocity = new Vector2(rb.velocity.x, hangingJumpForce);
                isHanging = false;
            }
            if (crouchPressed) { // 悬挂下落
                rb.bodyType = RigidbodyType2D.Dynamic;
                isHanging = false;
            }
        }
        // 单次跳跃
        if (jumpPressed && isOnGround && !isJump && !isHeadBlocked) {
            if (isCrouch) { // 下蹲跳跃加成
                StandUp();
                rb.AddForce(new Vector2(0f, crouchJumpBoost), ForceMode2D.Impulse);
            }
            isOnGround = false;
            isJump = true;
            jumpTime = Time.time + jumpHoldDuration; // 开始计算跳跃时间 = 当前跳跃时刻 + 长按时间，Time.time 是在不断增长的，eg：在游戏开始后的第 15s，按下跳跃，jumpTime = 15.1
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse); // 给一个向上的冲力(突然发生的)
            AudioManager.PlayJumpAudio(); // 跳跃音效
        }
        // 长按跳跃
        else if (isJump) {
            if (jumpHold) {
                rb.AddForce(new Vector2(0f, jumpHoldForce), ForceMode2D.Impulse);
            }
            if (jumpTime < Time.time) { // 控制时效性，如果 jumpTime < 当前的实时时间，退出跳跃状态（使这所有添加力的函数都失效），否则长按跳跃时会一直浮在空中
                isJump = false;
            }
            //AudioManager.PlayJumpAudio(); // 跳跃音效
        }
    }
}
