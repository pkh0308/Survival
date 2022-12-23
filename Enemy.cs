using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    BoxCollider2D boxCol;
    SpriteRenderer spriteRenderer;

    bool isDie;
    WaitForSeconds dieSec;

    //스탯 관련
    int curHp;
    public int maxHp;

    public float moveSpeed;
    float moveDelta;

    public int meleePow;
    public int rangePow;
    WaitForSeconds atkSec;
    Coroutine atkRoutine;
    Player target;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCol = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        dieSec = new WaitForSeconds(1.0f);
    }

    void OnEnable()
    {
        boxCol.enabled = true;
        curHp = maxHp;
        isDie = false;
    }

    void Start()
    {
        curHp = maxHp;
        atkSec = new WaitForSeconds(0.1f);
    }

    void Update()
    {
        if (isDie) return;
        if (GameManager.IsPaused) return;
        
        Move();
        Flip();
    }

    void Move()
    {
        moveDelta = moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, Player.playerPos, moveDelta);
    }

    void Flip()
    {
        if (Player.playerPos.x < transform.position.x && !spriteRenderer.flipX)
            spriteRenderer.flipX = true;
        else if(Player.playerPos.x > transform.position.x && spriteRenderer.flipX)
            spriteRenderer.flipX = false;
    }

    //전투 관련
    public void OnDamaged(int dmg)
    {
        //데미지 표기
        UiManager.showDamage(dmg, transform.position);

        if (dmg >= curHp)
            StartCoroutine(OnDie());
        else
            curHp -= dmg;
    }

    //밀치는 효과가 있는 경우
    public void OnDamaged(int dmg, Vector3 vec)
    {
        //데미지 표기
        UiManager.showDamage(dmg, transform.position);

        if (dmg >= curHp)
            StartCoroutine(OnDie());
        else
            curHp -= dmg;

        rigid.AddForce(vec, ForceMode2D.Impulse);
        Invoke(nameof(StopForce), 0.3f);
    }

    void StopForce()
    {
        rigid.velocity = Vector3.zero;
    }

    IEnumerator OnDie(bool timeOver = false)
    {
        anim.SetTrigger("OnDie");
        isDie = true;
        boxCol.enabled = false;
        if(atkRoutine != null) StopCoroutine(atkRoutine);

        if(!timeOver) //타임 오버 외 사망(타임 오버는 드랍 x, 킬 카운트 x)
        {
            GameManager.killCountPlus();
            DropItem();
        }

        yield return dieSec;
        gameObject.SetActive(false);
    }

    void DropItem()
    {
        //확률 계산
        int val = Random.Range(0, 10000);

        if (val > 8000)      val = 2;
        else if (val > 5000) val = 1;
        else                 val = 0;

        GameObject gem = ObjectManager.dropExp(val);
        gem.transform.position = transform.position;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.CompareTag(Tags.player))
        {
            if (col.TryGetComponent<Player>(out target))
            {
                atkRoutine = StartCoroutine(Attack());
                return;
            }
        }
        //스테이지 클리어 시 처리(타임 아웃)
        if (col.gameObject.CompareTag(Tags.stageEnder))
        {
            StartCoroutine(OnDie(true));
            return;
        }
        //폭탄 습득 시 처리
        if (col.gameObject.CompareTag(Tags.bomb))
        {
            StartCoroutine(OnDie());
            return;
        }
    }

    IEnumerator Attack()
    {
        while(!target.IsDie)
        {
            target.OnDamaged(meleePow);
            yield return atkSec;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.CompareTag(Tags.player))
        {
            StopCoroutine(atkRoutine);
        }
    }
}
