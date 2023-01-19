using System.Collections;
using UnityEngine;

public class PoisonFly : Enemy
{
    [SerializeField] float tripleShotInterval;

    protected override void BossRoutine()
    {
        StartCoroutine(TripleShot());
    }

    //플레이어를 방향과 거기서 +30도, -30도 회전 시킨 방향으로 3발 발사
    IEnumerator TripleShot()
    {
        float count = 0;
        while (!isDie)
        {
            if (GameManager.IsPaused)
            {
                yield return null;
                continue;
            }

            count += Time.deltaTime;
            if (count > tripleShotInterval)
            {
                GameObject[] bullets = new GameObject[3];
                for (int i = 0; i < bullets.Length; i++)
                {
                    bullets[i] = ObjectManager.makeEnemyBullet(bulletId);
                    bullets[i].transform.position = transform.position;
                }
                Vector3 dirVec = (Player.playerPos - transform.position).normalized;
                bullets[0].GetComponent<EnemyBullet>().Shoot(rangePow, dirVec);
                bullets[1].GetComponent<EnemyBullet>().Shoot(rangePow, Quaternion.Euler(0, 0, 30) * dirVec);
                bullets[2].GetComponent<EnemyBullet>().Shoot(rangePow, Quaternion.Euler(0, 0, -30) * dirVec);

                count = 0;
            }
            yield return null;
        }
    }
}