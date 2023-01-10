using UnityEngine;

public class Defender : WeaponBase
{
    [SerializeField] float knuckbackOffset;

    protected override void IndividualInitialize()
    {
        //별도로 구현할 내용 X
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
        if(col.CompareTag(Tags.enemyBullet))
        {
            col.gameObject.SetActive(false);
            return;
        }
        //적 피격 로직 호출
        col.GetComponent<Enemy>().OnDamaged((int)(weaponData.WeaponAtk * atkPower), 
                                            (col.transform.position - Player.playerPos).normalized * knuckbackOffset);
        AcmDmg((int)(weaponData.WeaponAtk * atkPower));
    }
}