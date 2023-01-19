using System.Collections;
using UnityEngine;

//무기 투사체들의 공통 로직 클래스
//각 무기 클래스는 해당 클래스를 상속받아 사용
//무기별 특징은 IndividualInitialize()에서 구현
public class WeaponBase : MonoBehaviour
{
    protected WeaponData weaponData;

    protected Rigidbody2D rigid;
    protected Sprite sprite;
    protected SpriteRenderer spriteRenderer;
    protected Collider2D coll;
    protected Animator anim;
    protected Vector3 direction;

    //일시정지 대응
    protected bool isPaused;
    protected Vector2 initialVelocity;

    //기본 스케일
    protected Vector3 initialScale;
    protected Vector3 initialColliderScale;

    //캐릭터 스탯
    protected float atkPower;
    protected float atkScale;
    protected float projSpeed;
    protected float atkRemainTime;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<Sprite>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        TryGetComponent<Collider2D>(out coll);
        TryGetComponent<Animator>(out anim);
        direction = Vector3.zero;

        isPaused = false;
    }

    void OnEnable()
    {
        initialScale = transform.localScale;
        spriteRenderer.enabled = true;
        if (coll != null)
        {
            initialColliderScale = coll.transform.localScale;
            coll.enabled = true;
        } 
    }

    void Update()
    {
        //일시정지 시작
        if (GameManager.IsPaused && !isPaused)
        {
            Pause();
            return;
        }
        //일시정지 종료
        if (!GameManager.IsPaused && isPaused)
        {
            PauseOff();
        }
    }

    void Pause()
    {
        isPaused = true;
        initialVelocity = rigid.velocity;
        rigid.velocity = Vector2.zero;
    }

    void PauseOff()
    {
        isPaused = false;
        rigid.velocity = initialVelocity;
    }

    void OnDisable()
    {
        transform.localScale = initialScale;
        if(coll != null)  
            coll.transform.localScale = initialColliderScale;
        transform.rotation = Quaternion.identity;

        spriteRenderer.flipY = false;
    }

    public void Initialize(WeaponData data, float power, float scale, float speed, float time) 
    {
        weaponData = data;
        atkPower = power;
        atkScale = scale;
        projSpeed = speed;
        atkRemainTime = time;

        transform.localScale *= atkScale;
        if(coll != null) coll.transform.localScale *= atkScale;

        IndividualInitialize();

        if (weaponData.WeaponRemainTime > 0)
            StartCoroutine(TimeOver());
    }

    protected void AcmDmg(int val)
    {
        Weapons.accumulateDmg(weaponData.WeaponId, val);
    }

    protected virtual IEnumerator TimeOver()
    {
        float timer = 0;
        float timeLimit = weaponData.WeaponRemainTime * atkRemainTime;
        while (gameObject.activeSelf)
        {
            if(GameManager.IsPaused)
            {
                yield return null;
                continue;
            }

            timer += Time.deltaTime;
            if(timer > timeLimit)
            {
                gameObject.SetActive(false);
                yield break;
            }
            yield return null;
        }
    }

    //필수 개별 구현 함수
    protected virtual void IndividualInitialize() { }
}