using UnityEngine;

public class Defender : WeaponBase
{
    protected override void IndividualInitialize()
    {
        StageSoundManager.playWeaponSfx((int)StageSoundManager.WeaponSfx.defender);
    }

    //회전을 위해 update 함수 사용
    void Update()
    {
        transform.RotateAround(Player.playerPos, Vector3.forward, weaponData.WeaponProjectileSpeed * Time.deltaTime);

        if (GameManager.IsPaused) gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag(Tags.enemy) && !col.CompareTag(Tags.enemyBullet)) return;

        //적 탄환 제거
        if(col.CompareTag(Tags.enemyBullet))
            col.gameObject.SetActive(false);

        col.GetComponent<Enemy>().OnDamaged((int)(weaponData.WeaponAtk * atkPower), 
                                            (col.transform.position - Player.playerPos).normalized * 5.0f);
        AcmDmg((int)(weaponData.WeaponAtk * atkPower));
    }
}
