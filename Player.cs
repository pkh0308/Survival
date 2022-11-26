using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] UiManager uiManager;

    public static Vector3 playerPos;

    public int characterId;
    public float moveSpeed;
    Vector3 moveVec;

    Weapons weaponLogic;
    SpriteRenderer spriteRender;
    Animator anim;

    //전투 관련
    int curHp;
    public int maxHp;
    bool isDie;
    public bool IsDie { get { return isDie; } }

    float powerRate;
    public float PowerRate { get { return powerRate; } }

    //기본 무기 타입
    [SerializeField] int basicWeaponId;

    void Awake()
    {
        spriteRender = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        weaponLogic = GetComponent<Weapons>();

        moveVec = Vector3.zero;
    }

    // Start is called before the first frame update
    void Start()
    {
        curHp = maxHp;
        powerRate = 1.0f;
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

        transform.position += moveSpeed * Time.deltaTime * moveVec;
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
