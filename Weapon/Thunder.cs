using UnityEngine;
using System.Collections;

public class Thunder : WeaponBase
{
    protected override void IndividualInitialize()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 15.0f, LayerMask.GetMask("Enemy"));
        if (cols.Length == 0)
        {
            gameObject.SetActive(false);
            return;
        }

        int idx = Random.Range(0, cols.Length);
        StartCoroutine(Attack(cols[idx]));
    }

    IEnumerator Attack(Collider2D col)
    {
        transform.position = col.transform.position;
        col.TryGetComponent<Enemy>(out Enemy enemyLogic);
        if (enemyLogic != null)
        {
            int dmg = (int)(weaponData.WeaponAtk * atkPower);
            enemyLogic.OnDamaged(dmg);
            Weapons.accumulateDmg(weaponData.WeaponId, dmg);
        } 
        StageSoundManager.playWeaponSfx((int)StageSoundManager.WeaponSfx.thunder);

        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }
}