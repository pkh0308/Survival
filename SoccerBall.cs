using UnityEngine;

public class SoccerBall : WeaponBase
{
    protected override void IndividualInitialize()
    {
        direction.x = Random.Range(-5.0f, 5.0f);
        direction.y = Random.Range(-5.0f, 5.0f);
        direction = direction.normalized * weaponData.WeaponProjectileSpeed;
        rigid.AddForce(direction, ForceMode2D.Impulse);

        StageSoundManager.playWeaponSfx((int)StageSoundManager.WeaponSfx.soccerBall);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag(Tags.enemy)) return;

        col.GetComponent<Enemy>().OnDamaged((int)(weaponData.WeaponAtk * atkPower));
        StageSoundManager.playWeaponSfx((int)StageSoundManager.WeaponSfx.soccerBall);

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
