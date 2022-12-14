using UnityEngine;

public class Shuriken : WeaponBase
{
    protected override void IndividualInitialize()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 15.0f, LayerMask.GetMask("Enemy"));
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
            Rotate(cols[minIdx].gameObject.transform.position);
        }
        else
        {
            direction = Vector3.right;
            transform.rotation = Quaternion.Euler(0, 0, -90);
        }

        rigid.AddForce(direction * weaponData.WeaponProjectileSpeed * projSpeed, ForceMode2D.Impulse);
        StageSoundManager.playWeaponSfx((int)StageSoundManager.WeaponSfx.shuriken);
    }

    //타겟과 플레이어간 각도를 arctan로 계산하여 회전
    void Rotate(Vector3 target)
    {
        float diff_x = target.x - transform.position.x;
        float diff_y = target.y - transform.position.y;
        if (diff_x == 0) diff_x = 0.01f; // DevideByZero 방지
        
        float angle = Mathf.Atan(diff_x / diff_y) * Mathf.Rad2Deg * -1;
        Vector3 rotationVec = Vector3.forward * angle;
        transform.Rotate(rotationVec);

        //타겟의 y좌표가 플레이어보다 아래쪽일 경우 뒤집기
        if (diff_y < 0) spriteRenderer.flipY = true;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag(Tags.enemy)) return;

        col.GetComponent<Enemy>().OnDamaged((int)(weaponData.WeaponAtk * atkPower));

        gameObject.SetActive(false);
    }
}
