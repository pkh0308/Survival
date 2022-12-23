﻿using System;
using UnityEngine;

public class StageSoundManager : MonoBehaviour
{
    AudioSource bgmAudioSource;
    AudioSource[] sfxAudioSources;
    AudioSource curSfxSource;
    
    //재생 함수 호출용 열거형
    public enum StageBgm { stage_1 = 100 }
    public enum StageSfx { getExp = 1000, levelUp, stageClear, gameOver, meat_or_magnet, gold, bomb }
    public enum WeaponSfx { soccerBall = 1100, shuriken, defender }

    //bgm
    [SerializeField] AudioClip stage_1;

    //sfx
    [SerializeField] AudioClip getExp;
    [SerializeField] AudioClip levelUp;
    [SerializeField] AudioClip stageClear;
    [SerializeField] AudioClip gameOver;
    [SerializeField] AudioClip meat_or_magnet;
    [SerializeField] AudioClip gold;
    [SerializeField] AudioClip bomb;

    //weapon
    public static Action<int> playWeaponSfx; 
    [SerializeField] AudioClip soccerBallSfx;
    [SerializeField] AudioClip shurikenSfx;
    [SerializeField] AudioClip defenderSfx;

    void Awake()
    {
        playWeaponSfx = (a) => { PlayWeaponSfx(a); };

        AudioSource[] sources = GetComponentsInChildren<AudioSource>();
        bgmAudioSource = sources[0];
        sfxAudioSources = new AudioSource[sources.Length - 1];
        for (int i = 1; i < sources.Length; i++)
            sfxAudioSources[i - 1] = sources[i];
    }

    void Start()
    {
        bgmAudioSource.loop = true;
        foreach(AudioSource sfx in sfxAudioSources)
            sfx.loop = false;

        PlayBgm((int)StageBgm.stage_1);
    }

    public void PlayBgm(int idx)
    {
        switch (idx)
        {
            case (int)StageBgm.stage_1:
                bgmAudioSource.clip = stage_1;
                bgmAudioSource.Play();
                break;
        }
    }

    public void PlaySfx(int idx)
    {
        //재생중이지 않은 오디오 소스 선택
        for(int i = 0; i < sfxAudioSources.Length; i++)
        {
            if (sfxAudioSources[i].isPlaying) continue;

            curSfxSource = sfxAudioSources[i];
            break;
        }
        //전부 재생중일 경우 마지막 소스 사용
        if (curSfxSource == null) 
            curSfxSource = sfxAudioSources[sfxAudioSources.Length - 1];

        switch (idx)
        {
            case (int)StageSfx.getExp:
                curSfxSource.clip = getExp;
                curSfxSource.Play();
                break;
            case (int)StageSfx.stageClear:
                bgmAudioSource.Stop();
                curSfxSource.clip = stageClear;
                curSfxSource.Play();
                break;
            case (int)StageSfx.gameOver:
                bgmAudioSource.Stop();
                curSfxSource.clip = gameOver;
                curSfxSource.Play();
                break;
            case (int)StageSfx.meat_or_magnet:
                curSfxSource.clip = meat_or_magnet;
                curSfxSource.Play();
                break;
            case (int)StageSfx.gold:
                curSfxSource.clip = gold;
                curSfxSource.Play();
                break;
            case (int)StageSfx.bomb:
                curSfxSource.clip = bomb;
                curSfxSource.Play();
                break;
        }
        curSfxSource = null; //재생 후 null로 초기화
    }

    public void PlayWeaponSfx(int idx)
    {
        //재생중이지 않은 오디오 소스 선택
        for (int i = 0; i < sfxAudioSources.Length; i++)
        {
            if (sfxAudioSources[i].isPlaying) continue;

            curSfxSource = sfxAudioSources[i];
            break;
        }
        //전부 재생중일 경우 마지막 소스 사용
        if (curSfxSource == null)
            curSfxSource = sfxAudioSources[sfxAudioSources.Length - 1];

        switch (idx)
        {
            case (int)WeaponSfx.soccerBall:
                curSfxSource.clip = soccerBallSfx;
                curSfxSource.volume = 0.5f;
                curSfxSource.Play();
                break;
            case (int)WeaponSfx.shuriken:
                curSfxSource.clip = shurikenSfx;
                curSfxSource.Play();
                break;
            case (int)WeaponSfx.defender:
                curSfxSource.clip = defenderSfx;
                curSfxSource.volume = 0.7f;
                curSfxSource.Play();
                break;
        }
        //재생 후 초기화
        curSfxSource.volume = 1.0f;
        curSfxSource = null; 
    }
}
