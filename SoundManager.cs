using System;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    AudioSource bgmAudioSource;
    AudioSource[] sfxAudioSources;
    AudioSource curSfxSource;
    
    //재생 함수 호출용 열거형
    public enum StageBgm { lobbyBgm, stage_1, lotteryBgm, lotteryStart }
    public enum Sfx { btnClick, statUpgrade, getExp, levelUp, stageClear, gameOver, meat_or_magnet, gold, bomb, lotteryEnd, bossAlert, playerDamaged, playerDeath }
    public enum WeaponSfx { soccerBall, shuriken, defender, missile, thunder, explodeMine, explosion, defenderAttack }

    //bgm
    public static Action<StageBgm> playBgm;
    public static Action stopBgm;
    AudioClip[] bgmArr;  

    //sfx
    public static Action<Sfx> playSfx;
    AudioClip[] sfxArr;

    //weapon
    public static Action<WeaponSfx> playWeaponSfx;
    AudioClip[] weaponSfxArr;

    // 로드 체크용
    int loadingCount = 0;

    #region 초기화
    void Awake()
    {
        Initialize();
    }

    void Initialize()
    {
        // 대리자 초기화
        playBgm = (a) => { PlayBgm(a); };
        stopBgm = () => { StopBgm(); };
        playSfx = (a) => { PlaySfx(a); };
        playWeaponSfx = (a) => { PlayWeaponSfx(a); };

        // 첫 AudioSource는 bgm, 나머지는 sfx용으로 설정
        AudioSource[] sources = GetComponentsInChildren<AudioSource>();
        bgmAudioSource = sources[0];
        sfxAudioSources = new AudioSource[sources.Length - 1];
        for (int i = 1; i < sources.Length; i++)
            sfxAudioSources[i - 1] = sources[i];

        // 배열 초기화
        bgmArr = new AudioClip[Enum.GetNames(typeof(StageBgm)).Length];
        sfxArr = new AudioClip[Enum.GetNames(typeof(Sfx)).Length];
        weaponSfxArr = new AudioClip[Enum.GetNames(typeof(WeaponSfx)).Length];

        // 루프 설정 
        bgmAudioSource.loop = true;
        foreach (AudioSource sfx in sfxAudioSources)
            sfx.loop = false;
    }

    void Start()
    {
        LoadAudioClips();
    }

    void LoadAudioClips()
    {
        LoadAudio(typeof(StageBgm));
        LoadAudio(typeof(Sfx));
        LoadAudio(typeof(WeaponSfx));
    }

    void LoadAudio(Type type)
    {
        AudioClip[] arr;
        if (type == typeof(StageBgm))
            arr = bgmArr;
        else if (type == typeof(Sfx))
            arr = sfxArr;
        else
            arr = weaponSfxArr;

        string[] names = Enum.GetNames(type);
        for(int i = 0; i < arr.Length; i++)
        {
            loadingCount++;
            ResourceManager.Instance.GetResourceByIdx<AudioClip>(names[i], i, (audio, idx) => 
            { 
                arr[idx] = audio; 
                CheckAudioLoading(); 
            });
        }
    }

    // 로딩 완료 시 로비 입장
    void CheckAudioLoading()
    {
        loadingCount--;
        if (loadingCount == 0)
            LoadingSceneManager.Inst.LoadLobby();
    }
    #endregion

    #region 재생 및 정지
    public void PlayBgm(StageBgm bgm, float volume = 1.0f)
    {
        int idx = Convert.ToInt32(bgm);
        if(idx < 0 || idx >= bgmArr.Length)
        {
            Debug.Log($"Wrong bgmIdx: {idx}");
            return;
        }

        bgmAudioSource.clip = bgmArr[idx];
        bgmAudioSource.volume = volume;
        bgmAudioSource.Play();
    }

    public void StopBgm()
    {
        bgmAudioSource.Stop();
    }

    public void PlaySfx(Sfx sfx, float volume = 1.0f)
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

        int idx = Convert.ToInt32(sfx);
        if (idx < 0 || idx >= sfxArr.Length)
        {
            Debug.Log($"Wrong sfxIdx: {idx}");
            return;
        }

        curSfxSource.clip = sfxArr[idx];
        curSfxSource.volume = volume;
        curSfxSource.Play();

        curSfxSource = null; //재생 후 null로 초기화
    }

    public void PlayWeaponSfx(WeaponSfx weaponSfx, float volume = 1.0f)
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

        int idx = Convert.ToInt32(weaponSfx);
        if (idx < 0 || idx >= weaponSfxArr.Length)
        {
            Debug.Log($"Wrong weaponSfxIdx: {idx}");
            return;
        }

        curSfxSource.clip = weaponSfxArr[idx];
        curSfxSource.volume = volume;
        curSfxSource.Play();

        //재생 후 초기화
        curSfxSource = null; 
    }
    #endregion
}