using System.Collections;
using UnityEngine;

//�÷��̾� Ÿ�� or Ÿ�ӿ��� �� �� ���� ����
public class PoisonShot : EnemyBullet
{
    protected override void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag(Tags.player))
            return;

        col.GetComponent<Player>().OnDamaged(dmg);
        GameObject poison = ObjectManager.makeEnemyBullet(ObjectNames.poisonArea);
        poison.transform.position = transform.position;
        gameObject.SetActive(false);
    }

    protected override IEnumerator TimeOver()
    {
        float count = 0;
        while (gameObject.activeSelf)
        {
            if (isPaused) //�Ͻ����� ������ ������ ���
            {
                yield return null;
                continue;
            }
            count += Time.deltaTime;
            if (count > timeOutSec)
            {
                GameObject poison = ObjectManager.makeEnemyBullet(ObjectNames.poisonArea);
                poison.transform.position = transform.position;
                gameObject.SetActive(false);
            }
            yield return null;
        }
    }
}