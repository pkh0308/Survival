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

    //�Ͻ����� ����
    bool isPaused;
    Vector2 rigidVelocity;

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
        rigidVelocity = rigid.velocity;
        rigid.velocity = Vector2.zero;
    }

    void PauseOff()
    {
        isPaused = false;
        rigid.velocity = rigidVelocity;
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
        yield return new WaitForSeconds(timeOutSec);
        gameObject.SetActive(false);
    }

    void OnDisable()
    {
        transform.rotation = Quaternion.identity;
    }
}