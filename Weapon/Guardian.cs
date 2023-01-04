using System.Collections;
using UnityEngine;

public class Guardian : WeaponBase
{
    protected override void IndividualInitialize()
    {
        //������ ������ ���� X
    }

    //Ÿ�� ���� ����
    protected override IEnumerator TimeOver()
    {
        //������ ������ ���� X
        yield break;
    }

    //ȸ���� ���� update �Լ� ���
    void Update()
    {
        if (GameManager.IsPaused) return;

        transform.RotateAround(Player.playerPos, Vector3.forward, weaponData.WeaponProjectileSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag(Tags.enemy) && !col.CompareTag(Tags.enemyBullet)) return;

        //�� źȯ ����
        if (col.CompareTag(Tags.enemyBullet))
            col.gameObject.SetActive(false);

        col.GetComponent<Enemy>().OnDamaged((int)(weaponData.WeaponAtk * atkPower),
                                            (col.transform.position - Player.playerPos).normalized * 5.0f);
        AcmDmg((int)(weaponData.WeaponAtk * atkPower));
    }
}