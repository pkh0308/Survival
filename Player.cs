using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    //플레이어 위치 추적용
    public static Vector3 playerPos;

    GameManager gameManager;
    UiManager uiManager;
    Weapons weaponLogic;

    Rigidbody2D rigid;
    SpriteRenderer spriteRender;
    Animator anim;

    float moveSpeed;
    Vector3 moveVec;
    Vector3 targetPos;

    //물리 컨트롤
    Coroutine velocityControllRoutine;
    [SerializeField] float velocityCutTime;

    //캐릭터 스탯
    CharacterData characterData;
    PlayerStatus stat;
    public static Action<PlayerStatus, int> updateStatus;

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
        rigid = GetComponent<Rigidbody2D>();

        getHeal = (healValue) => { GetHeal(healValue); };
        updateStatus = (status, id) => { UpdateStatus(status, id); };

        stat = PlayerStatusManager.getStatus();
        moveVec = Vector3.zero;
        targetPos = Vector3.forward;
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

    void Update()
    {
        playerPos = transform.position;

        InputCheck();
        VelocityControll();
    }

    void InputCheck()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            gameManager.Pause_Board();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            uiManager.InputEnter();
            return;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            uiManager.InputUpDown(KeyCode.UpArrow);
            return;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            uiManager.InputUpDown(KeyCode.DownArrow);
            return;
        }
    }

    void VelocityControll()
    {
        if (rigid.velocity == Vector2.zero) return;

        if (velocityControllRoutine == null)
            StartCoroutine(VelocityCut());
    }

    IEnumerator VelocityCut()
    {
        float count = 0;
        while (gameObject.activeSelf)
        {
            if (GameManager.IsPaused)
            {
                yield return null;
                continue;
            }
            count += Time.deltaTime;
            if (count > velocityCutTime)
            {
                rigid.velocity = Vector2.zero;
                velocityControllRoutine = null;
                yield break;
            }
            yield return null;
        }
    }

    void FixedUpdate()
    {
        if (isDie) return;

        if (GameManager.IsPaused == false)
        {
            Move();
            MouseMove();
        }
    }

    void Move()
    {
        moveVec.x = Input.GetAxisRaw("Horizontal");
        moveVec.y = Input.GetAxisRaw("Vertical");

        if (moveVec.x < 0) spriteRender.flipX = true;
        else if(moveVec.x > 0) spriteRender.flipX = false;

        rigid.transform.Translate(moveSpeed * stat.PlayerMoveSpeedVal * Time.fixedDeltaTime * moveVec);
    }

    void MouseMove()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (Input.GetMouseButton(0))
        {
            targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPos.z = 0;

            if (targetPos.x < transform.position.x) spriteRender.flipX = true;
            else if (targetPos.x > transform.position.x) spriteRender.flipX = false;
        }
        
        if (targetPos == Vector3.forward) return;
        
        transform.position =  Vector3.MoveTowards(transform.position, targetPos, moveSpeed * stat.PlayerMoveSpeedVal * Time.fixedDeltaTime);
        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
            targetPos = Vector3.forward;
    }

    //스테이터스 정보 갱신
    void UpdateStatus(PlayerStatus data, int id)
    {
        stat = data;
        if (id == ObjectNames.acc_heart)
            UpdateMaxHealth();
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

    void UpdateMaxHealth()
    {
        int beforeMaxHp = maxHp;
        maxHp = (int)(baseHp * stat.PlayerHealthVal);
        curHp += maxHp - beforeMaxHp; //증가한 최대 체력만큼 현재 체력도 증가
        uiManager.UpdateHp(curHp, maxHp);
    }

    public void GetHeal(int healVal)
    {
        healVal += curHp;
        curHp = healVal > maxHp ? maxHp : healVal;
        uiManager.UpdateHp(curHp, maxHp);
    }

    public void OnDamaged(int dmg, bool dotDmg = false)
    {
        if (isDie) return; //이미 죽은 경우 스킵

        if(!dotDmg) //도트 데미지가 아니라면 방어력 만큼 데미지 감소
            dmg -= stat.PlayerdefVal;

        if(curHp <= dmg)
        {
            curHp = 0;
            uiManager.UpdateHp(curHp, maxHp);
            OnDie();
            return;
        }

        curHp -= dmg;
        uiManager.UpdateHp(curHp, maxHp);
        StageSoundManager.playSfx((int)StageSoundManager.StageSfx.playerDamaged);
    }

    void OnDie()
    {
        isDie = true;
        weaponLogic.StopAllCoroutines();
        StageSoundManager.playSfx((int)StageSoundManager.StageSfx.playerDeath);
        anim.SetTrigger("OnDie");
        gameManager.PlayerDie();
    }
}