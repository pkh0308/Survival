using System.Collections;
using UnityEngine;
using MyMath;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] float projSpeed;
    [SerializeField] float timeOutSec;
    int dmg;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;

    //일시정지 대응
    bool isPaused;
    Vector2 beforeVelocity;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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
        beforeVelocity = rigid.velocity;
        rigid.velocity = Vector2.zero;
    }

    void PauseOff()
    {
        isPaused = false;
        rigid.velocity = beforeVelocity;
    }

    public void Shoot(int dmg, Vector3 dir)
    {
        this.dmg = dmg;

        rigid.AddForce(dir * projSpeed, ForceMode2D.Impulse);
        transform.Rotate(MyRotation.Rotate(dir, out bool flip));
        spriteRenderer.flipY = flip;

        StartCoroutine(TimeOver());
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag(Tags.player))
            return;

        col.GetComponent<Player>().OnDamaged(dmg);
        gameObject.SetActive(false);
    }

    IEnumerator TimeOver()
    {
        float count = 0;
        while(gameObject.activeSelf)
        {
            if(isPaused) //일시정지 상태인 동안은 대기
            {
                yield return null;
                continue;
            }
            count += Time.deltaTime;
            if(count > timeOutSec)
                gameObject.SetActive(false);
            yield return null;
        }
    }

    void OnDisable()
    {
        transform.rotation = Quaternion.identity;
    }
}