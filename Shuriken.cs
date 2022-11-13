using UnityEngine;

public class Shuriken : MonoBehaviour
{
    WeaponData weaponData;

    Rigidbody2D rigid;
    Vector3 direction;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        direction = Vector3.one;
    }

    public void Initialize(WeaponData data)
    {
        weaponData = data;

        SearchTarget();
        Invoke(nameof(TimeOver), weaponData.WeaponCooltime);
    }

    void SearchTarget()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 15.0f, LayerMask.GetMask("Enemy"));Debug.Log(cols.Length);
        if (cols.Length > 0)
            direction = (cols[0].gameObject.transform.position - transform.position).normalized;
        else
            direction = Vector3.right;

        rigid.AddForce(direction * weaponData.WeaponProjectileSpeed, ForceMode2D.Impulse);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag(Tags.enemy)) return;

        col.GetComponent<Enemy>().OnDamaged(weaponData.WeaponAtk);

        gameObject.SetActive(false);
    }

    void TimeOver()
    {
        gameObject.SetActive(false);
    }
}
