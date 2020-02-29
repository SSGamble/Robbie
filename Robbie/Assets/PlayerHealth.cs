using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 死亡机制
/// </summary>
public class PlayerHealth : MonoBehaviour {

    // 为什么不用 Tag？用层做伤害机制，有助于解决后面的很多问题，比如说让角色无敌
    int trapLayer;
    public GameObject deathVFXPre;

    void Start() {
        trapLayer = LayerMask.NameToLayer("Traps");
    }

    void Update() {

    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.layer == trapLayer) {
            Instantiate(deathVFXPre, transform.position, transform.rotation);
            gameObject.SetActive(false);
            AudioManager.PlayDeathAudio();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // 重新加载场景
        }
    }
}
