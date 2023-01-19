using System.Collections;
using UnityEngine;
using MyMath;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] protected float projSpeed;
    [SerializeField] protected float timeOutSec;
    protected int dmg;
    protected Rigidbody2D rigid;
    protected SpriteRenderer spriteRenderer;

    //�Ͻ����� ����
    protected bool isPaused;
    protected Vector2 beforeVelocity;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        //�Ͻ����� ����
        if (GameManager.IsPaused && !isPaused)
        {
            Pause();
            return;
        }
        //�Ͻ����� ����
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

    //�Է¹��� �������� ��ô
    public void Shoot(int dmg, Vector3 dir)
    {
        this.dmg = dmg;

        rigid.AddForce(dir * projSpeed, ForceMode2D.Impulse);
        transform.Rotate(MyRotation.Rotate(dir, out bool flip));
        spriteRenderer.flipY = flip;

        StartCoroutine(TimeOver());
    }

    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag(Tags.player))
            return;

        col.GetComponent<Player>().OnDamaged(dmg);
        gameObject.SetActive(false);
    }

    //���� �ð� ���� �� ��Ȱ��ȭ
    protected virtual IEnumerator TimeOver()
    {
        float count = 0;
        while(gameObject.activeSelf)
        {
            if(isPaused) //�Ͻ����� ������ ������ ���
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