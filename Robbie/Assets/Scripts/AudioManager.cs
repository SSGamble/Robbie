using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 音效管理
/// </summary>
public class AudioManager : MonoBehaviour {
    // 单例
    static AudioManager current;

    [Header("环境声音")]
    public AudioClip ambientClip;
    public AudioClip musicClip;

    [Header(" Robbie音效")]
    public AudioClip[] walkStepClips; // 含有 4 个声音片段
    public AudioClip[] crouchStepClips;
    public AudioClip jumpClip;
    public AudioClip jumpVoiceClip;
    public AudioClip deathClip;
    public AudioClip deathVoiceClip;

    [Header(" FX 特效")]
    public AudioClip deathFXClip; // 死亡后，归还金币的声音

    AudioSource ambientSource; // 环境音
    AudioSource musicSource; // BGM
    AudioSource fxSource;
    AudioSource playerSource;
    AudioSource voiceSource;
    AudioSource deathSource;

    private void Awake() {
        if (current != null) {
            Destroy(gameObject);
            return;
        }
        current = this;
        DontDestroyOnLoad(gameObject);

        ambientSource = gameObject.AddComponent<AudioSource>();

        musicSource = gameObject.AddComponent<AudioSource>();
        fxSource = gameObject.AddComponent<AudioSource>();
        playerSource = gameObject.AddComponent<AudioSource>();
        voiceSource = gameObject.AddComponent<AudioSource>();

        StartLevelAudio();
    }

    private void StartLevelAudio() {
        current.ambientSource.clip = current.ambientClip;
        current.ambientSource.loop = true;
        current.ambientSource.Play();

        current.musicSource.clip = current.musicClip;
        current.musicSource.loop = true;
        current.musicSource.Play();
    }

    /// <summary>
    /// 播放走路的音效
    /// </summary>
    public static void PlayFootStepAudio() {
        int index = Random.Range(0, current.walkStepClips.Length);
        current.playerSource.clip = current.walkStepClips[index];
        current.playerSource.Play();
    }

    /// <summary>
    /// 播放下蹲时走路的音效
    /// </summary>
    public static void PlayCrouchFootStepAudio() {
        int index = Random.Range(0, current.crouchStepClips.Length);
        current.playerSource.clip = current.crouchStepClips[index];
        current.playerSource.Play();
    }

    /// <summary>
    /// 播放跳跃的音效
    /// </summary>
    public static void PlayJumpAudio() {
        current.playerSource.clip = current.jumpClip;
        current.playerSource.Play();

        current.voiceSource.clip = current.jumpVoiceClip;
        current.voiceSource.Play();
    }

    /// <summary>
    /// 播放跳跃的音效
    /// </summary>
    public static void PlayDeathAudio() {
        current.playerSource.clip = current.deathClip;
        current.playerSource.Play();

        current.voiceSource.clip = current.deathVoiceClip;
        current.voiceSource.Play();

        current.fxSource.clip = current.deathFXClip;
        current.fxSource.Play();
    }
}
