using UnityEngine;
using System.Collections;

public class ExplodeMine : WeaponBase
{
    protected override void IndividualInitialize()
    {
        direction.x = Random.Range(-5.0f, 5.0f);
        direction.y = Random.Range(-5.0f, 5.0f);
        transform.position = direction;

        SoundManager.playWeaponSfx(SoundManager.WeaponSfx.explodeMine);
    }

    //적에게 닿을 시 Explode() 호출
    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag(Tags.enemy)) return;

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
                    StartCoroutine(Explode());
              
                yield break;
            }
            yield return null;
        }
    }

    //투사체 콜라이더 비활성화 후 playerBullet 생성, 애니메이션 시간 대기 후 비활성화
    IEnumerator Explode()
    {
        coll.enabled = false;
        anim.SetTrigger("OnFoot");
        
        yield return new WaitForSeconds(0.5f);
        GameObject explosion = ObjectManager.makeObj(ObjectNames.explosion);
        explosion.GetComponent<Explosion>().Initialize((int)(weaponData.WeaponAtk * atkPower), weaponData.WeaponId, weaponData.WeaponScale * atkScale);
        explosion.transform.position = transform.position;
        explosion.SetActive(true);
        SoundManager.playWeaponSfx(SoundManager.WeaponSfx.explosion);

        gameObject.SetActive(false);
    }
}