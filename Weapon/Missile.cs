using UnityEngine;
using System.Collections;
using MyMath;

public class Missile : WeaponBase
{
    Vector3 targetPos;

    protected override void IndividualInitialize()
    {
        //무작위 방향으로 발사
        direction.x = Random.Range(-5.0f, 5.0f);
        direction.y = Random.Range(-5.0f, 5.0f);
        direction = direction.normalized * weaponData.WeaponProjectileSpeed;
        rigid.AddForce(direction, ForceMode2D.Impulse);

        initialVelocity = rigid.velocity;
        transform.Rotate(MyRotation.Rotate(direction));

        SoundManager.playWeaponSfx(SoundManager.WeaponSfx.missile);
    }

    //적에게 닿을 시 Explode() 호출
    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag(Tags.enemy)) return;

        targetPos = col.transform.position;
        StartCoroutine(Explode());
    }
    //지속시간 종료 시 Explode() 호출
    protected override IEnumerator TimeOver()
    {
        float timer = 0;
        float timeLimit = weaponData.WeaponRemainTime * atkRemainTime;
        while (gameObject.activeSelf)
        {
            if (GameManager.IsPaused)
            {
                yield return null;
                continue;
            }

            timer += Time.deltaTime;
            if (timer > timeLimit)
            {
                if (coll.enabled) //아직 터지지 않았다면 실행
                {
                    targetPos = transform.position;
                    StartCoroutine(Explode());
                }
                yield break;
            }
            yield return null;
        }
    }

    //투사체 콜라이더 비활성화 후 playerBullet 생성, 애니메이션 시간 대기 후 비활성화
    IEnumerator Explode()
    {
        coll.enabled = false;
        rigid.velocity = Vector2.zero;
        spriteRenderer.enabled = false;
        SoundManager.playWeaponSfx(SoundManager.WeaponSfx.explosion);

        GameObject explosion = ObjectManager.makeObj(ObjectNames.explosion);
        explosion.GetComponent<Explosion>().Initialize((int)(weaponData.WeaponAtk * atkPower), weaponData.WeaponId, weaponData.WeaponScale * atkScale);
        //미사일과 부딪힌 타겟 사이의 지점에서 폭발
        explosion.transform.position = (transform.position + targetPos) / 2;
        explosion.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }
}