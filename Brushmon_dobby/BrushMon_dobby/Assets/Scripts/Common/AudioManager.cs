using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoSingleton<AudioManager> {
    private AudioSource[] audioSources;
    private string languagePath;
    private string characterPath;


    void Awake () {
        gameObject.AddComponent<AudioSource> ();
        gameObject.AddComponent<AudioSource> ();
        gameObject.AddComponent<AudioSource> ();
        audioSources = GetComponents<AudioSource> ();

        LoadAudio();
    }

    public void LoadAudio()
    {
        SystemLanguage language = LocalizationManager.Instance.LoadVoice();

        switch (language)
        {
            case SystemLanguage.Korean:
                languagePath = "kr/";
                break;

            case SystemLanguage.English:
                languagePath = "en/";
                break;

            case SystemLanguage.ChineseTraditional:
                languagePath = "tw/";
                break;

            case SystemLanguage.Japanese:
                languagePath = "jp/";
                break;

            case SystemLanguage.ChineseSimplified:
                languagePath = "tw/";
                break;

            //case SystemLanguage.ChineseSimplified:
            //    languagePath = "cn/";
            //    break;

            default:
                languagePath = "en/";
                break;
        }
    }

    public void playAudio(string clipName, Action onAudioComplete, float beforeDelay = 0f, float afterDelay = 0f)
    {
        //LogManager.Log("playAudio : " + clipName);

        if (audioSources[1].isPlaying) audioSources[1].Stop();

        PreStickerVo preStickerVo = ConfigurationData.Instance.GetValueFromJson<PreStickerVo>("PreSticker");

        AudioClip clip;

        /* 그린몰드 사운드 공용으로 사용하기 위한 소스
        if (clipName.IndexOf("greenmold") > -1)
        {
            clip = Resources.Load<AudioClip>("Audio/common/" + clipName);
            LogManager.Log("Audio/" + languagePath + characterPath + clipName);
            StartCoroutine(playAudio(clip, onAudioComplete, beforeDelay, afterDelay));
        }
        else if (clipName.IndexOf("story") > -1)
        */
        if (clipName.IndexOf("greenmold") > -1 || clipName.IndexOf("story") > -1)
        {
            characterPath = "";
            clip = Resources.Load<AudioClip>("Audio/" + languagePath + characterPath + clipName);
        }
        else if (preStickerVo != null && preStickerVo.name != null)
        {
            if (preStickerVo.name.IndexOf("cheese") > -1)
            {
                characterPath = "cheese/";
            }
            else if (preStickerVo.name.IndexOf("cherry") > -1)
            {
                characterPath = "cherry/";
            }
            else if (preStickerVo.name.IndexOf("soda") > -1)
            {
                characterPath = "soda/";
            }

            clip = Resources.Load<AudioClip>("Audio/" + languagePath + characterPath + clipName);
        }
        else
        {
            LogManager.LogWarning("playAudio Warning : " + clipName);
            characterPath = "cheese/";
            clip = Resources.Load<AudioClip>("Audio/" + languagePath + characterPath + clipName);
        }
        
        StartCoroutine(playAudio(clip, onAudioComplete, beforeDelay, afterDelay));
    }

    IEnumerator playAudio (AudioClip clip, Action onAudioComplete, float beforeDelay = 0f, float afterDelay = 0f) {
        yield return new WaitForSeconds (beforeDelay);
        audioSources[1].PlayOneShot (clip);
        yield return new WaitForSeconds (clip.length);
        yield return new WaitForSeconds (afterDelay);
        if (onAudioComplete != null)
            onAudioComplete ();
    }

    public void playBGM (string clipName, bool isLoop = true) {
        if (audioSources[0].isPlaying) audioSources[0].Stop ();
        AudioClip clip = Resources.Load<AudioClip> ("Audio/common/" + clipName);
        audioSources[0].loop = isLoop;
        audioSources[0].clip = clip;
        audioSources[0].Play ();
    }

    public void playEffect (string clipName, float beforeDelay = 0) {
        if (audioSources[2].isPlaying) audioSources[2].Stop ();
        AudioClip clip = Resources.Load<AudioClip> ("Audio/common/" + clipName);
        StartCoroutine (playEffect (clip, beforeDelay));
    }

    public void playEffectNotStop (string clipName, float beforeDelay = 0) {
        if (audioSources[2].isPlaying) return;
        AudioClip clip = Resources.Load<AudioClip> ("Audio/common/" + clipName);
        audioSources[2].PlayOneShot (clip);
    }

    IEnumerator playEffect (AudioClip clip, float beforeDelay) {
        yield return new WaitForSeconds (beforeDelay);
        audioSources[2].PlayOneShot (clip);
    }

    public bool isPlayingBGM (string clipName) {
        AudioClip clip = audioSources[0].clip;
        return clip == null?false: (clip.name.Equals (clipName) ? audioSources[0].isPlaying : false);
    }

    public void Pause (bool Pause) {
        foreach (AudioSource audioSource in audioSources) {
            if (Pause)
                audioSource.Pause ();
            else
                audioSource.UnPause ();
        }
    }

    public void StopAll () {
        foreach (AudioSource audioSource in audioSources) {
            audioSource.Stop ();
        }
    }

    public void StopEffect (string clipName) {
        if (audioSources[2] != null && audioSources[2].clip != null && audioSources[2].clip.name.Equals (clipName))
            audioSources[2].Stop ();
    }

    public void MuteAll (bool isMute) {
        foreach (AudioSource audioSource in audioSources) {
            audioSource.mute = isMute;
        }
    }

}