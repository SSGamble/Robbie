using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色动画
/// </summary>
public class PlayerAnimation : MonoBehaviour {
    Animator anim;
    PlayerMovement movement;
    Rigidbody2D rb;

    // 编号
    int groundID;
    int hangingID;
    int crouchID;
    int speedID;
    int fallID;

    void Start() {
        anim = GetComponent<Animator>();
        movement = GetComponentInParent<PlayerMovement>();
        rb = GetComponentInParent<Rigidbody2D>();
        groundID = Animator.StringToHash("isOnGround"); // 字符转编号
        hangingID = Animator.StringToHash("isHanging"); 
        crouchID = Animator.StringToHash("isCrouching"); 
        speedID = Animator.StringToHash("speed");
        fallID = Animator.StringToHash("verticalVelocity");
    }

    void Update() {
        anim.SetFloat(speedID, Mathf.Abs(movement.xVelocity));
        //anim.SetBool("isOnGround", movement.isOnGround);
        anim.SetBool(groundID, movement.isOnGround); // 推荐用编号进行操作，因为有些平台字符型会出现问题
        anim.SetBool(hangingID, movement.isHanging);
        anim.SetBool(crouchID, movement.isCrouch);
        anim.SetFloat(fallID, rb.velocity.y);
    }

    /// <summary>
    /// 走路的声音
    /// </summary>
    public void StepAudio() {
        AudioManager.PlayFootStepAudio();
    }

    /// <summary>
    /// 下蹲走路的声音
    /// </summary>
    public void CrouchStepAudio() {
        AudioManager.PlayCrouchFootStepAudio();
    }
}
