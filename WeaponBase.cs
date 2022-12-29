using UnityEngine;
using System.Collections;

public class WeaponBase : MonoBehaviour
{
    protected WeaponData weaponData;

    protected Rigidbody2D rigid;
    protected Sprite sprite;
    protected SpriteRenderer spriteRenderer;
    protected Collider2D coll;
    protected Animator anim;
    protected Vector3 direction;

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

        Invoke(nameof(TimeOver), weaponData.WeaponRemainTime * atkRemainTime);
    }

    protected void AcmDmg(int val)
    {
        Weapons.accumulateDmg(weaponData.WeaponId, val);
    }

    //투사체 회전용 함수
    //타겟의 위치값을 Vector3 형태로 받아 플레이어와의 각도를 arctan로 계산하여 회전
    protected void Rotate(Vector3 target)
    {
        float diff_x = target.x - transform.position.x;
        float diff_y = target.y - transform.position.y;
        if (diff_x == 0) diff_x = 0.01f; // DevideByZero 방지

        float angle = Mathf.Atan(diff_x / diff_y) * Mathf.Rad2Deg * -1;
        Vector3 rotationVec = Vector3.forward * angle;
        transform.Rotate(rotationVec);

        //타겟의 y좌표가 플레이어보다 아래쪽일 경우 뒤집기
        if (diff_y < 0) spriteRenderer.flipY = true;
    }

    protected virtual void TimeOver()
    {
        if (weaponData.WeaponRemainTime == 0) return;

        gameObject.SetActive(false);
    }

    //필수 개별 구현 함수
    protected virtual void IndividualInitialize() { }

}