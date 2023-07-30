using System;
using UnityEngine;

//UI 이미지 갱신용 저장소
public class SpriteManager: MonoBehaviour
{
    // 캐릭터
    public enum Character
    {
        kkurugi,
        ninja,
        knight,
        farmer,
        hunter,
        monk
    }
    Sprite[] sprite_Character = new Sprite[Enum.GetNames(typeof(Weapon)).Length];
    const int CHARACTER_INIT_ID = ObjectNames.kkurugi;

    // 일반 무기
    public enum Weapon
    {
        soccerBall,
        shuriken,
        defender,
        missile,
        thunder,
        explodeMine
    }
    Sprite[] sprite_Weapon = new Sprite[Enum.GetNames(typeof(Weapon)).Length];
    const int WEAPON_INIT_ID = ObjectNames.soccerBall;

    // 업그레이드 무기
    public enum UpgradedWeapon
    {
        quantumBall,
        shadowEdge,
        guardian,
        sharkMissile,
        judgement,
        hellfireMine
    }
    Sprite[] sprite_UpgradedWeapon = new Sprite[Enum.GetNames(typeof(UpgradedWeapon)).Length];
    const int UPGRADED_WEAPON_INIT_ID = ObjectNames.quantumBall;

    // 악세사리
    public enum Accessory
    {
        acc_bullet,
        acc_fuel,
        acc_accelerator,
        acc_energyCube,
        acc_extraBullet,
        acc_exoskeleton,
        acc_heart,
        acc_armor,
        acc_sneakers
    }
    Sprite[] sprite_Acc = new Sprite[Enum.GetNames(typeof(Accessory)).Length];
    const int ACCESSORY_INIT_ID = ObjectNames.acc_bullet;

    // 기타 아이템
    public enum ExtraItem
    {
        meat_50,
        gold_70
    }
    Sprite[] sprite_Extra = new Sprite[Enum.GetNames(typeof(ExtraItem)).Length];
    const int EXTRA_ITEM_INIT_ID = ObjectNames.meat_50;

    // UI
    public enum UI
    {
        weaponSlotNormal,
        weaponSlotLegandary,
        blackStar,
        normalStar,
        legandaryStar
    }
    Sprite[] sprite_UI = new Sprite[Enum.GetNames(typeof(UI)).Length];
    const int UI_INIT_ID = ObjectNames.weaponSlotNormal;


    public static Func<int, Sprite> getSprite;

    void Awake()
    {
        getSprite = (id) => { return GetSprite(id); };
    }

    void Start()
    {
        LoadSprites(typeof(Character));
        LoadSprites(typeof(Weapon));
        LoadSprites(typeof(UpgradedWeapon));
        LoadSprites(typeof(Accessory));
        LoadSprites(typeof(ExtraItem));
        LoadSprites(typeof(UI));
    }

    void LoadSprites(Type type)
    {
        Sprite[] sprites;
        if (type == typeof(Character))
            sprites = sprite_Character;
        else if (type == typeof(Weapon))
            sprites = sprite_Weapon;
        else if (type == typeof(UpgradedWeapon))
            sprites = sprite_UpgradedWeapon;
        else if (type == typeof(Accessory))
            sprites = sprite_Acc;
        else if (type == typeof(ExtraItem))
            sprites = sprite_Extra;
        else if (type == typeof(UI))
            sprites = sprite_UI;
        else
        {
            Debug.Log($"Wrong Type: {type}");
            return;
        }

        string[] names = Enum.GetNames(type);
        for(int i = 0; i < names.Length; i++)
        {
            ResourceManager.Instance.GetResourceByIdx<Sprite>(names[i], i, (op, idx) =>
            {
                sprites[idx] = op;
            });
        }
    }

    Sprite GetSprite(int id)
    {
        int idx = CheckID(id, out Sprite[] sprites);
        if(sprites == null)
        {
            Debug.Log($"Wrong ID: {id}");
            return null;
        }

        return sprites[idx];
    }

    int CheckID(int id, out Sprite[] sprites)
    {
        int result = -1;
        sprites = null;

        // 끝자리가 9로 끝나면 업그레이드 무기
        if(id % 10 == 9)
        {
            sprites = sprite_UpgradedWeapon;
            result = (id % 100) / 10;
        }
        // 1000 미만 - 강화 화면
        else if(id < 1000)
        {
            sprites = sprite_Acc;
            result = (id / 100) - 1;
        }
        // 1000대 - 캐릭터
        else if(id < 2000)
        {
            sprites = sprite_Character;
            result = id - CHARACTER_INIT_ID;
        }
        // 2000대 - 무기
        else if(id < 3000)
        {
            sprites = sprite_Weapon;
            result = (id % 100) / 10;
        }
        // 3000대 - 악세사리
        else if (id < 4000)
        {
            sprites = sprite_Acc;
            result = (id % 100) / 10;
        }
        // 8000 ~ 9000대 UI 
        else if (id < 10000)
        {
            sprites = sprite_Extra;
            result = (id % 100) / 10;
        }
        // 10000 이상은 UI
        else if(id > 10000)
        {
            sprites = sprite_UI;
            result = id - UI_INIT_ID;
        }

        return result;
    }
}
