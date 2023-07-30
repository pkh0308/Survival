using UnityEngine;

public class SoccerBall : WeaponBase
{
    protected override void IndividualInitialize()
    {
        direction.x = Random.Range(-5.0f, 5.0f);
        direction.y = Random.Range(-5.0f, 5.0f);
        direction = direction.normalized * weaponData.WeaponProjectileSpeed;
        rigid.AddForce(direction, ForceMode2D.Impulse);
        initialVelocity = rigid.velocity;

        SoundManager.playWeaponSfx((int)SoundManager.WeaponSfx.soccerBall);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag(Tags.enemy)) return;

        col.GetComponent<Enemy>().OnDamaged((int)(weaponData.WeaponAtk * atkPower));
        AcmDmg((int)(weaponData.WeaponAtk * atkPower));
        SoundManager.playWeaponSfx((int)SoundManager.WeaponSfx.soccerBall);

        //적에게 부딪힌 뒤 튕겨져 나갈 방향 계산
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
