using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoccerBall : MonoBehaviour
{
    Rigidbody2D rigid;

    Vector3 direction;

    WeaponData weaponData;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        direction = Vector3.zero;
    }

    public void Initialize(WeaponData data)
    {
        weaponData = data;

        direction.x = Random.Range(-5.0f, 5.0f);
        direction.y = Random.Range(-5.0f, 5.0f);
        direction = direction.normalized * weaponData.WeaponProjectileSpeed;
        rigid.AddForce(direction, ForceMode2D.Impulse);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag(Tags.enemy)) return;

        col.GetComponent<Enemy>().OnDamaged(weaponData.WeaponAtk);

        Vector3 diff = col.transform.position - transform.position;
        if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
        {
            direction.x *= -1;
            rigid.velocity = Vector3.zero;
            rigid.AddForce(direction, ForceMode2D.Impulse);
        }
        else
        {
            direction.y *= -1;
            rigid.velocity = Vector3.zero;
            rigid.AddForce(direction, ForceMode2D.Impulse);
        }
    }

}
