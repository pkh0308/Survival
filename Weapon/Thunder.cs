using UnityEngine;
using System.Collections;

public class Thunder : WeaponBase
{
    [SerializeField] float searchDistance;

    protected override void IndividualInitialize()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, searchDistance, LayerMask.GetMask("Enemy"));
        if (cols.Length == 0)
        {
            StartCoroutine(Attack());
            return;
        }

        int idx = Random.Range(0, cols.Length);
        StartCoroutine(Attack(cols[idx]));
    }

    IEnumerator Attack(Collider2D col = null)
    {
        if(col == null) //대상이 없을 경우 플레이어 자리에 낙뢰
        {
            transform.position = Player.playerPos;
        }
        else
        {
            transform.position = col.transform.position;
            col.TryGetComponent<Enemy>(out Enemy enemyLogic);
            if (enemyLogic != null)
            {
                int dmg = (int)(weaponData.WeaponAtk * atkPower);
                enemyLogic.OnDamaged(dmg);
                Weapons.accumulateDmg(weaponData.WeaponId, dmg);
            }
        }
        StageSoundManager.playWeaponSfx((int)StageSoundManager.WeaponSfx.thunder);

        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }
}