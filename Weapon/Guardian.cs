using System.Collections;
using UnityEngine;

public class Guardian : WeaponBase
{
    [SerializeField] float knuckbackOffset;

    protected override void IndividualInitialize()
    {
        //별도로 구현할 내용 X
    }

    //타임 오버 제거
    protected override IEnumerator TimeOver()
    {
        //별도로 구현할 내용 X
        yield break;
    }

    //회전을 위해 update 함수 사용
    void Update()
    {
        if (GameManager.IsPaused) return;

        transform.RotateAround(Player.playerPos, Vector3.forward, weaponData.WeaponProjectileSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag(Tags.enemy) && !col.CompareTag(Tags.enemyBullet)) return;

        //적 탄환 제거
        if (col.CompareTag(Tags.enemyBullet))
        {
            col.gameObject.SetActive(false);
            return;
        }
        //적 타격(넉백 포함)
        StageSoundManager.playWeaponSfx((int)StageSoundManager.WeaponSfx.defenderAttack);
        col.GetComponent<Enemy>().OnDamaged((int)(weaponData.WeaponAtk * atkPower),
                                            (col.transform.position - Player.playerPos).normalized * knuckbackOffset);
        AcmDmg((int)(weaponData.WeaponAtk * atkPower));
    }
}