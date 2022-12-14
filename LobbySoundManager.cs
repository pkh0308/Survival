using UnityEngine;

public class LobbySoundManager : MonoBehaviour
{
    AudioSource bgmAudioSource;
    AudioSource sfxAudioSource;

    public enum LobbyBgm { mainBg = 100 }
    public enum LobbySfx { btnClick  = 1000, upgrade }

    //bgm
    [SerializeField] AudioClip mainBg;

    //sfx
    [SerializeField] AudioClip btnClick;
    [SerializeField] AudioClip upgrade;

    void Awake()
    {
        AudioSource[] sources = GetComponentsInChildren<AudioSource>();
        bgmAudioSource = sources[0];
        sfxAudioSource = sources[1];
    }

    void Start()
    {
        bgmAudioSource.loop = true;
        sfxAudioSource.loop = false;

        PlayBgm((int)LobbyBgm.mainBg);
    }

    public void PlayBgm(int idx)
    {
        switch(idx)
        {
            case (int)LobbyBgm.mainBg:
                bgmAudioSource.clip = mainBg;
                bgmAudioSource.Play();
                break;
        }
    }

    public void PlaySfx(int idx)
    {
        switch (idx)
        {
            case (int)LobbySfx.btnClick:
                sfxAudioSource.clip = btnClick;
                sfxAudioSource.Play();
                break;
            case (int)LobbySfx.upgrade:
                sfxAudioSource.clip = upgrade;
                sfxAudioSource.Play();
                break;
        }
    }
}
