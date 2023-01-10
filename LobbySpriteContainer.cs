using UnityEngine;

public class LobbySpriteContainer : MonoBehaviour
{
    //강화 아이콘
    [SerializeField] Sprite image_101;
    [SerializeField] Sprite image_201;
    [SerializeField] Sprite image_301;
    [SerializeField] Sprite image_401;
    [SerializeField] Sprite image_501;
    [SerializeField] Sprite image_601;
    [SerializeField] Sprite image_701;
    [SerializeField] Sprite image_801;
    [SerializeField] Sprite image_901;

    //플레이어블 캐릭터
    [SerializeField] Sprite player_1001;
    [SerializeField] Sprite player_1002;
    [SerializeField] Sprite player_1003;
    [SerializeField] Sprite player_1004;
    [SerializeField] Sprite player_1005;
    [SerializeField] Sprite player_1006;

    //무기
    [SerializeField] Sprite soccerBall;
    [SerializeField] Sprite shuriken;
    [SerializeField] Sprite defender;
    [SerializeField] Sprite missile;
    [SerializeField] Sprite thunder;
    [SerializeField] Sprite explodeMine;

    public Sprite GetEnhancementSprite(int id)
    {
        switch(id)
        {
            case 101:
                return image_101;
            case 201:
                return image_201;
            case 301:
                return image_301;
            case 401:
                return image_401;
            case 501:
                return image_501;
            case 601:
                return image_601;
            case 701:
                return image_701;
            case 801:
                return image_801;
            case 901:
                return image_901;
            default:
                return null;
        }    
    }

    public Sprite GetPlayerSprite(int id)
    {
        switch (id)
        {
            case ObjectNames.kkurugi:
                return player_1001;
            case ObjectNames.ninja:
                return player_1002;
            case ObjectNames.knight:
                return player_1003;
            case ObjectNames.farmer:
                return player_1004;
            case ObjectNames.hunter:
                return player_1005;
            case ObjectNames.monk:
                return player_1006;
            default:
                return null;
        }
    }

    public Sprite GetWeaponSprite(int id)
    {
        switch (id)
        {
            case ObjectNames.soccerBall:
                return soccerBall;
            case ObjectNames.shuriken:
                return shuriken;
            case ObjectNames.defender:
                return defender;
            case ObjectNames.missile:
                return missile;
            case ObjectNames.thunder:
                return thunder;
            case ObjectNames.explodeMine:
                return explodeMine;
            default:
                return null;
        }
    }
}