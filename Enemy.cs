using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected Rigidbody2D rigid;
    protected Animator anim;
    protected BoxCollider2D boxCol;
    protected SpriteRenderer spriteRenderer;
    protected Coroutine velocityControllRoutine;
    protected float velocityCutTime;
    
    //몬스터 타입
    public enum enemyType { Normal, Unique, Boss }
    [SerializeField] enemyType type;

    protected bool isDie;
    protected WaitForSeconds dieSec;
    [SerializeField] protected float timeForDie;

    //스탯 관련
    protected int curHp;
    public int maxHp;

    public float moveSpeed;
    protected float moveDelta;

    //일시정지 대응
    bool isPaused;
    Vector2 rigidVelocity;

    //근접 공격(접촉 시)
    public int meleePow;
    protected WaitForSeconds meleeAtkSec;
    protected Coroutine meleeAtkRoutine;

    //원거리 공격
    public int rangePow;
    protected WaitForSeconds rangeAtkSec;
    protected Coroutine rangeAtkRoutine;
    [SerializeField] protected float rangeAtkInterval;
    [SerializeField] protected int bulletId;

    protected Player target;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCol = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        dieSec = new WaitForSeconds(timeForDie);
    }

    void OnEnable()
    {
        boxCol.enabled = true;
        curHp = maxHp;
        isDie = false;
    }

    void Start()
    {
        meleeAtkSec = new WaitForSeconds(0.15f);
        rangeAtkSec = new WaitForSeconds(rangeAtkInterval);
        velocityCutTime = 0.5f;

        if (rangePow > 0)
            rangeAtkRoutine = StartCoroutine(RangeAttack());
        if (type == enemyType.Boss)
            BossRoutine();
    }

    //보스 타입 개별 구현
    protected virtual void BossRoutine() { }

    void Update()
    {
        if (isDie) return;

        //일시정지 시작
        if (GameManager.IsPaused)
        {
            if(!isPaused) Pause();
            return;
        }
        //일시정지 종료
        if (isPaused)
        {
            PauseOff();
        }
        
        Move();
        Flip();
        VelocityControll();
    }

    void Move()
    {
        if (moveSpeed == 0) 
            return;

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

    void VelocityControll()
    {
        if (rigid.velocity == Vector2.zero) return;

        if(velocityControllRoutine == null) 
            velocityControllRoutine = StartCoroutine(VelocityCut());
    }

    IEnumerator VelocityCut()
    {
        yield return new WaitForSeconds(velocityCutTime);
        rigid.velocity = Vector2.zero;
        velocityControllRoutine = null;
    }

    void Pause()
    {
        isPaused = true;
        rigidVelocity = rigid.velocity;
        anim.speed = 0;
    }

    void PauseOff()
    {
        isPaused = false;
        rigid.velocity = rigidVelocity;
        anim.speed = 1;
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

        if (type == enemyType.Normal) //일반 몹만 밀치기 적용
        {
            rigid.AddForce(vec, ForceMode2D.Impulse);
        } 
    }

    IEnumerator OnDie(bool timeOver = false)
    {
        anim.SetTrigger("OnDie");
        isDie = true;
        boxCol.enabled = false;
        if(meleeAtkRoutine != null) StopCoroutine(meleeAtkRoutine);
        if(rangeAtkRoutine != null) StopCoroutine(rangeAtkRoutine);

        if (!timeOver) //타임 오버 외 사망(타임 오버는 드랍 x, 킬 카운트 x)
        {
            GameManager.killCountPlus();
            DropItem();
        }

        yield return dieSec;
        gameObject.SetActive(false);
        if (type == enemyType.Boss)
            GameManager.bossDie();
    }

    void DropItem()
    {
        //네임드일 경우 보물상자 드랍
        if(type == enemyType.Unique)
        {
            GameObject box = ObjectManager.makeObj(ObjectNames.treasureBox);
            box.transform.position = transform.position;
            return;
        }

        //확률 계산
        int val = Random.Range(0, 10000);

        if (val > 8000)      val = 2;
        else if (val > 5000) val = 1;
        else                 val = 0;

        GameObject gem = ObjectManager.dropExp(val);
        gem.transform.position = transform.position;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag(Tags.player))
        {
            if (col.gameObject.TryGetComponent<Player>(out target))
            {
                meleeAtkRoutine = StartCoroutine(MeleeAttack());
                return;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        //스테이지 클리어 시 처리(타임 아웃)
        if (col.CompareTag(Tags.stageEnder))
        {
            StartCoroutine(OnDie(true));
            return;
        }
        //폭탄 습득 시 처리(네임드, 보스 제외)
        if (col.CompareTag(Tags.bomb) && type == enemyType.Normal)
        {
            StartCoroutine(OnDie());
            return;
        }
    }

    IEnumerator MeleeAttack()
    {
        while(!GameManager.IsPaused)
        {
            if (target.IsDie) yield break;

            target.OnDamaged(meleePow);
            yield return meleeAtkSec;
        }
    }

    IEnumerator RangeAttack()
    {
        while(!GameManager.IsPaused)
        {
            if (isDie) yield break;
            yield return rangeAtkSec;

            GameObject bullet = ObjectManager.makeEnemyBullet(bulletId);
            bullet.transform.position = transform.position;
            bullet.GetComponent<EnemyBullet>().Shoot(rangePow, (Player.playerPos - transform.position).normalized);
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag(Tags.player))
        {
            if(meleeAtkRoutine != null)
                StopCoroutine(meleeAtkRoutine);
        }
    }
}