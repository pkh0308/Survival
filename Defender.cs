using UnityEngine;

public class Defender : MonoBehaviour
{
    WeaponData weaponData;

    Rigidbody2D rigid;
    Vector3 offset;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        offset = Vector3.zero;
    }

    public void Initialize(WeaponData data)
    {
        weaponData = data;

        Invoke(nameof(TimeOver), weaponData.WeaponCooltime);

        transform.RotateAround(Player.playerPos, Vector3.forward, 360.0f);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag(Tags.enemy)) return;

        //적 탄환 제거
        if(col.CompareTag(Tags.enemyBullet))
            col.gameObject.SetActive(false);

        col.GetComponent<Enemy>().OnDamaged(weaponData.WeaponAtk);

        gameObject.SetActive(false);
    }

    void TimeOver()
    {
        gameObject.SetActive(false);
    }
}
