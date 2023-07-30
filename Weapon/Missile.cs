using UnityEngine;
using System.Collections;
using MyMath;

public class Missile : WeaponBase
{
    Vector3 targetPos;

    protected override void IndividualInitialize()
    {
        //������ �������� �߻�
        direction.x = Random.Range(-5.0f, 5.0f);
        direction.y = Random.Range(-5.0f, 5.0f);
        direction = direction.normalized * weaponData.WeaponProjectileSpeed;
        rigid.AddForce(direction, ForceMode2D.Impulse);

        initialVelocity = rigid.velocity;
        transform.Rotate(MyRotation.Rotate(direction));

        SoundManager.playWeaponSfx(SoundManager.WeaponSfx.missile);
    }

    //������ ���� �� Explode() ȣ��
    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag(Tags.enemy)) return;

        targetPos = col.transform.position;
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
                {
                    targetPos = transform.position;
                    StartCoroutine(Explode());
                }
                yield break;
            }
            yield return null;
        }
    }

    //����ü �ݶ��̴� ��Ȱ��ȭ �� playerBullet ����, �ִϸ��̼� �ð� ��� �� ��Ȱ��ȭ
    IEnumerator Explode()
    {
        coll.enabled = false;
        rigid.velocity = Vector2.zero;
        spriteRenderer.enabled = false;
        SoundManager.playWeaponSfx(SoundManager.WeaponSfx.explosion);

        GameObject explosion = ObjectManager.makeObj(ObjectNames.explosion);
        explosion.GetComponent<Explosion>().Initialize((int)(weaponData.WeaponAtk * atkPower), weaponData.WeaponId, weaponData.WeaponScale * atkScale);
        //�̻��ϰ� �ε��� Ÿ�� ������ �������� ����
        explosion.transform.position = (transform.position + targetPos) / 2;
        explosion.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }
}