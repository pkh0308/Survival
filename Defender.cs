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
    }

    void Update()
    {
        transform.RotateAround(Player.playerPos, Vector3.forward, weaponData.WeaponProjectileSpeed * Time.deltaTime);

        if (GameManager.IsPaused) gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag(Tags.enemy)) return;

        //적 탄환 제거
        if(col.CompareTag(Tags.enemyBullet))
            col.gameObject.SetActive(false);

        col.GetComponent<Enemy>().OnDamaged(weaponData.WeaponAtk, (col.transform.position - Player.playerPos).normalized * 5.0f);
    }

    void TimeOver()
    {
        gameObject.SetActive(false);
    }
}
