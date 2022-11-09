using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponData
{
    //아이디
    int weaponId;
    public int WeaponId { get { return weaponId; } }
    //이름
    string weaponName;
    public string WeaponName { get { return weaponName; } }
    //레벨
    int weaponLevel;
    public int WeaponLevel { get { return weaponLevel; } }
    //공격력
    int weaponAtk;
    public int WeaponAtk { get { return weaponAtk; } }
    //쿨타임
    float weaponCooltime;
    public float WeaponCooltime { get { return weaponCooltime; } }
    //범위(크기)
    float weaponScale;
    public float WeaponScale { get { return weaponScale; } }
    //투사체 수
    int weaponProjectileCount;
    public int WeaponProjectileCount { get { return weaponProjectileCount; } }
    //투사체 속도
    float weaponProjectileSpeed;
    public float WeaponProjectileSpeed { get { return weaponProjectileSpeed; } }
    //설명
    string weaponDescription;
    public string WeaponDescription { get { return weaponDescription; } }

    public WeaponData(int id, string name, int lv, int atk, float cooltime, float scale, int count, float speed, string desc)
    {
        weaponId = id;
        weaponName = name;
        weaponLevel = lv;
        weaponAtk = atk;
        weaponCooltime = cooltime;
        weaponScale = scale;
        weaponProjectileCount = count;
        weaponProjectileSpeed = speed;
        weaponDescription = desc;
    }

    public virtual void Initialize() { }
}
