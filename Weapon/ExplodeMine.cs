using UnityEngine;
using System.Collections;

public class ExplodeMine : WeaponBase
{
    protected override void IndividualInitialize()
    {
        direction.x = Random.Range(-5.0f, 5.0f);
        direction.y = Random.Range(-5.0f, 5.0f);
        transform.position = direction;

        StageSoundManager.playWeaponSfx((int)StageSoundManager.WeaponSfx.explodeMine);
    }

    //������ ���� �� Explode() ȣ��
    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag(Tags.enemy)) return;

        StartCoroutine(Explode());
    }
    //���ӽð� ���� �� Explode() ȣ��
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
                if (coll.enabled) //���� ������ �ʾҴٸ� ����
                    StartCoroutine(Explode());
              
                yield break;
            }
            yield return null;
        }
    }

    //����ü �ݶ��̴� ��Ȱ��ȭ �� playerBullet ����, �ִϸ��̼� �ð� ��� �� ��Ȱ��ȭ
    IEnumerator Explode()
    {
        coll.enabled = false;
        anim.SetTrigger("OnFoot");
        
        yield return new WaitForSeconds(0.5f);
        GameObject playerBullet = ObjectManager.makeObj(ObjectNames.playerBullet);
        playerBullet.GetComponent<Explosion>().Initialize((int)(weaponData.WeaponAtk * atkPower), weaponData.WeaponId, weaponData.WeaponScale * atkScale);
        playerBullet.transform.position = transform.position;
        playerBullet.SetActive(true);
        StageSoundManager.playWeaponSfx((int)StageSoundManager.WeaponSfx.explosion);

        gameObject.SetActive(false);
    }
}