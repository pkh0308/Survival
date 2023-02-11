using UnityEngine;
using MyMath;

public class Shuriken : WeaponBase
{
    [SerializeField] float searchDistance;

    protected override void IndividualInitialize()
    {
        //가장 가까운 적을 탐색하여 발사
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, searchDistance, LayerMask.GetMask("Enemy")); 
        if (cols.Length > 0)
        {
            float min = 1000; 
            int minIdx = 0;
            for(int i = 0; i < cols.Length; i++)
            {
                float dist = Vector3.Distance(Player.playerPos, cols[i].transform.position);
                if (dist < min)
                {
                    min = dist;
                    minIdx = i;
                }
            }
            
            direction = (cols[minIdx].gameObject.transform.position - transform.position).normalized;
            transform.Rotate(MyRotation.Rotate(transform.position, cols[minIdx].gameObject.transform.position));
        }
        else //탐색 영역 내에 적이 없을 경우
        {
            direction = Vector3.right;
            transform.rotation = Quaternion.Euler(0, 0, -90);
        }

        rigid.AddForce(direction * weaponData.WeaponProjectileSpeed * projSpeed, ForceMode2D.Impulse);
        initialVelocity = rigid.velocity;
        StageSoundManager.playWeaponSfx((int)StageSoundManager.WeaponSfx.shuriken);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag(Tags.enemy)) return;

        col.GetComponent<Enemy>().OnDamaged((int)(weaponData.WeaponAtk * atkPower));
        AcmDmg((int)(weaponData.WeaponAtk * atkPower));

        gameObject.SetActive(false);
    }
}
