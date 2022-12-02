using UnityEngine;

public class Shuriken : WeaponBase
{
    protected override void IndividualInitialize()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 15.0f, LayerMask.GetMask("Enemy"));
        if (cols.Length > 0)
            direction = (cols[0].gameObject.transform.position - transform.position).normalized;
        else
            direction = Vector3.right;

        rigid.AddForce(direction * weaponData.WeaponProjectileSpeed * projSpeed, ForceMode2D.Impulse);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag(Tags.enemy)) return;

        col.GetComponent<Enemy>().OnDamaged((int)(weaponData.WeaponAtk * atkPower));

        gameObject.SetActive(false);
    }
}
