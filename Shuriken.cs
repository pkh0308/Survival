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
                float dist = Vector3.Distance(Player.playerPos, transform.position);
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
    }

    void Rotate(Vector3 target)
    {
        int offset = transform.position.y < target.y ? 1 : -1;
        float diff_x = target.x - transform.position.x;
        float diff_y = target.y - transform.position.y;
        if (diff_x == 0) diff_x = 0.01f; // DevideByZero 방지
        if (diff_x < 0) spriteRenderer.flipX = true;
        float angle = Mathf.Atan(diff_y / diff_x) * Mathf.Rad2Deg;
        
        Vector3 rotationVec = Vector3.forward * angle * offset;

        transform.Rotate(rotationVec);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag(Tags.enemy)) return;

        col.GetComponent<Enemy>().OnDamaged((int)(weaponData.WeaponAtk * atkPower));

        gameObject.SetActive(false);
    }
}
