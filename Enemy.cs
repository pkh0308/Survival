using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    BoxCollider2D boxCol;

    bool isDie;

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
    }

    private void OnEnable()
    {
        boxCol.enabled = true;
    }

    void Start()
    {
        curHp = maxHp;
        atkSec = new WaitForSeconds(0.1f);
    }

    void Update()
    {
        if (isDie) return;

        if(GameManager.IsPaused != true)
        {
            Move();
        }
    }

    void Move()
    {
        moveDelta = moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, Player.playerPos, moveDelta);
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

    IEnumerator OnDie()
    {
        //anim.SetTrigger("OnDie");
        isDie = true;
        boxCol.enabled = false;
        GameManager.killCountPlus();
        DropItem();
            
        yield return new WaitForSeconds(1.5f);
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
                atkRoutine = StartCoroutine(Attack());
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
