using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    //플레이어 위치 추적용
    public static Vector3 playerPos;

    GameManager gameManager;
    UiManager uiManager;
    CharacterData characterData;

    Weapons weaponLogic;
    SpriteRenderer spriteRender;
    Animator anim;

    float moveSpeed;
    Vector3 moveVec;

    //캐릭터 스탯
    PlayerStatus stat;

    //전투 관련
    int curHp;
    int baseHp;
    int maxHp;
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
        
        stat = PlayerStatusManager.getStatus();
        moveVec = Vector3.zero;
    }

    // Start is called before the first frame update
    void Start()
    {
        maxHp = (int)(baseHp * stat.PlayerHealthVal);
        curHp = maxHp;
        weaponLogic.GetWeapon(basicWeaponId);
    }

    // Update is called once per frame
    void Update()
    {
        playerPos = transform.position;

        if (isDie) return;

        InputCheck();
        if (GameManager.IsPaused == false)
        {
            Move();
        }
    }

    void InputCheck()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            gameManager.Pause();
        }
    }

    void Move()
    {
        moveVec.x = Input.GetAxisRaw("Horizontal");
        moveVec.y = Input.GetAxisRaw("Vertical");

        if (moveVec.x < 0) spriteRender.flipX = true;
        else if(moveVec.x > 0) spriteRender.flipX = false;
        
        transform.position += moveSpeed * stat.PlayerMoveSpeedVal * Time.deltaTime * moveVec;
    }

    //전투 관련
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag(Tags.expGem))
        {
            gameManager.ExpUp(col.GetComponent<ExpGem>().GetExp());
            return;
        }
    }

    public void OnDamaged(int dmg)
    {
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
