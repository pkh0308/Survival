using System.Collections;
using UnityEngine;

//플레이어 타격 or 타임오버 시 독 지대 생성
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
            if (isPaused) //일시정지 상태인 동안은 대기
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