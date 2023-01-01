﻿using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    //플레이어 위치 추적용
    public static Vector3 playerPos;

    GameManager gameManager;
    UiManager uiManager;

    Weapons weaponLogic;
    SpriteRenderer spriteRender;
    Animator anim;

    float moveSpeed;
    Vector3 moveVec;

    //캐릭터 스탯
    CharacterData characterData;
    PlayerStatus stat;

    //전투 관련
    int curHp;
    int baseHp;
    int maxHp;
    public static Action<int> getHeal;
    bool isDie;
    public bool IsDie { get { return isDie; } }

    //기본 무기 타입
    [SerializeField] int basicWeaponId;

    //ObjectManager에 호출하여 필요한 레퍼런스 제공
    public void Initialize(GameManager gameManager, UiManager uiManager, CharacterData data)
    {
        this.gameManager = gameManager;
        this.uiManager = uiManager;
        characterData = data;

        baseHp = characterData.playerHealth;
        moveSpeed = characterData.playerMoveSpeed;
    }

    void Awake()
    {
        spriteRender = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        weaponLogic = GetComponent<Weapons>();

        getHeal = (a) => { GetHeal(a); };

        stat = PlayerStatusManager.getStatus();
        moveVec = Vector3.zero;
    }

    // Start is called before the first frame update
    void Start()
    {
        StatusMerge();
        weaponLogic.SetStatus(stat);
        maxHp = (int)(baseHp * stat.PlayerHealthVal);
        curHp = maxHp;
        weaponLogic.GetWeapon(basicWeaponId);
    }

    //캐릭터 기본 스탯(CharacterData)와 강화 수치(PlayerStatus)를 합산
    //최대 체력과 이동속도는 제외
    void StatusMerge()
    {
        stat.AddStatus(nameof(stat.AtkPowerVal), characterData.atkPower);
        stat.AddStatus(nameof(stat.AtkScaleVal), characterData.atkScale); 
        stat.AddStatus(nameof(stat.ProjSpeedVal), characterData.projSpeed);
        stat.AddStatus(nameof(stat.CoolTimeVal), characterData.coolTime);
        stat.AddStatus(nameof(stat.ProjCountVal), characterData.projCount);
        stat.AddStatus(nameof(stat.AtkRemainTimeVal), characterData.atkRemainTime);
        stat.AddStatus(nameof(stat.PlayerdefVal), characterData.playerdef);
    }

    // Update is called once per frame
    void Update()
    {
        playerPos = transform.position;

        if (isDie) return;

        InputCheck();
    }

    void InputCheck()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            gameManager.Pause_Exit();
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            uiManager.InputEnter();
        }
    }

    void FixedUpdate()
    {
        if (isDie) return;

        if (GameManager.IsPaused == false)
        {
            Move();
        }
    }

    void Move()
    {
        moveVec.x = Input.GetAxisRaw("Horizontal");
        moveVec.y = Input.GetAxisRaw("Vertical");

        if (moveVec.x < 0) spriteRender.flipX = true;
        else if(moveVec.x > 0) spriteRender.flipX = false;
        
        transform.position += moveSpeed * stat.PlayerMoveSpeedVal * Time.fixedDeltaTime * moveVec;
    }

    //전투 관련
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag(Tags.item))
        {
            gameManager.GetItem(col.GetComponent<Item>().Consume());
            return;
        }
    }

    public void GetHeal(int healVal)
    {
        healVal += curHp;
        curHp = healVal > maxHp ? maxHp : healVal;
        uiManager.UpdateHp(curHp, maxHp);
    }

    public void OnDamaged(int dmg)
    {
        if (isDie) return; //이미 죽은 경우 스킵

        dmg -= stat.PlayerdefVal;

        if(curHp <= dmg)
        {
            curHp = 0;
            uiManager.UpdateHp(curHp, maxHp);
            StartCoroutine(OnDie());
            return;
        }

        curHp -= dmg;
        uiManager.UpdateHp(curHp, maxHp);
    }

    IEnumerator OnDie()
    {
        isDie = true;
        //anim.SetTrigger("OnDie");

        yield return new WaitForSeconds(1.5f);
        gameManager.GameOver();
    }
}