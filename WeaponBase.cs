﻿using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    protected WeaponData weaponData;

    protected Rigidbody2D rigid;
    protected  Sprite sprite;
    protected Collider2D coll;
    protected Vector3 direction;

    //캐릭터 스탯
    protected float atkPower;
    protected float atkScale;
    protected float projSpeed;
    protected float atkRemainTime;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<Sprite>();
        coll = GetComponent<Collider2D>();
        direction = Vector3.one;
    }
    
    public void Initialize(WeaponData data, float power, float scale, float speed, float time) 
    {
        weaponData = data;
        atkPower = power;
        atkScale = scale;
        projSpeed = speed;
        atkRemainTime = time;

        transform.localScale *= atkScale;
        coll.transform.localScale *= atkScale;

        IndividualInitialize();

        Invoke(nameof(TimeOver), weaponData.WeaponRemainTime * atkRemainTime);
    }

    //필수 개별 구현 함수
    protected virtual void IndividualInitialize() { }

    protected void TimeOver()
    {
        gameObject.SetActive(false);
    }
}