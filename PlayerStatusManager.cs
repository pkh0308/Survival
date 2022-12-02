using System;
using UnityEngine;

public class PlayerStatusManager : MonoBehaviour
{
    public static Action<PlayerStatus> setStatus;
    public static Func<PlayerStatus> getStatus;

    public static Action<int> setCharacterId;
    public static Func<int> getCharacterId;

    void Awake()
    {
        setStatus = (a) => { SetStatus(a); };
        getStatus = () => { return GetStatus(); };
        setCharacterId = (a) => { SetCharacterId(a); };
        getCharacterId = () => { return GetCharacterId(); };
    }

    //PlayerStatus 전달
    PlayerStatus playerStatus;

    void SetStatus(PlayerStatus status)
    {
        playerStatus = status;
    }

    PlayerStatus GetStatus()
    {
        return playerStatus;
    }

    //캐릭터 ID 전달
    int characterId;

    void SetCharacterId(int id)
    {
        characterId = id;
    }

    int GetCharacterId()
    {
        return characterId;
    }
}
