using System.Collections;
using UnityEngine;

public class MonsterTree : Enemy
{
    [SerializeField] float octoShootInterval;
    Vector3[] directions;

    protected override void BossRoutine()
    {
        StartCoroutine(OctoShoot());
    }

    IEnumerator OctoShoot()
    {
        directions = new Vector3[8];
        directions[0] = Vector3.up;
        directions[1] = Vector3.down;
        directions[2] = Vector3.left;
        directions[3] = Vector3.right;
        directions[4] = new Vector3(0.7f, 0.7f, 0);
        directions[5] = new Vector3(-0.7f, 0.7f, 0);
        directions[6] = new Vector3(0.7f, -0.7f, 0);
        directions[7] = new Vector3(-0.7f, -0.7f, 0);

        float count = 0;
        while (!isDie)
        {
            if(GameManager.IsPaused)
            {
                yield return null;
                continue;
            }

            count += Time.deltaTime;
            if(count > octoShootInterval)
            {
                for (int i = 0; i < directions.Length; i++)
                {
                    GameObject bullet = ObjectManager.makeEnemy(bulletId);
                    bullet.transform.position = transform.position;
                    bullet.GetComponent<EnemyBullet>().Shoot(rangePow, directions[i]);
                }
                count = 0;
            }
            yield return null;
        }
    }
}