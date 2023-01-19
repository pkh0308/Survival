using System.Collections;
using UnityEngine;
using MyMath;

public class BossPig : Enemy
{
    [SerializeField] float rushInterval;
    [SerializeField] float rushWarningInterval;
    [SerializeField] float rushEndInterval;
    GameObject direction;
    
    enum State { None, Warning, OnRush }
    State state;

    float beforeSpeed;
    [SerializeField] float rushSpeed;

    protected override void BossRoutine()
    {
        state = State.None;
        velocityCutTime = rushEndInterval; //러쉬 지속시간이 끝나면 rigid.velocity 초기화
        direction = ObjectManager.makeObj(ObjectNames.bossRushDir);
        direction.GetComponent<BossRushDir>().SetBoss(gameObject);
        direction.SetActive(false);
        StartCoroutine(Rush());
    }

    IEnumerator Rush()
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
            if (state == State.None) //러쉬 시작 전
            {
                if(count > rushInterval)
                {
                    count = 0;
                    state = State.Warning;
                    direction.SetActive(true);
                }
            }
            else if(state == State.Warning) //러쉬 방향 경고 중
            {
                if(count > rushWarningInterval)
                {
                    count = 0;
                    state = State.OnRush;
                    direction.SetActive(false);
                    beforeSpeed = moveSpeed;
                    moveSpeed = 0; //통상 이동 차단
                    rigid.AddForce((Player.playerPos - transform.position).normalized * rushSpeed, ForceMode2D.Impulse);
                }
            }
            else //러쉬 중
            {
                if (count > rushEndInterval)
                {
                    count = 0;
                    state = State.None;
                    moveSpeed = beforeSpeed;
                }
            }
            yield return null;
        }
    }

    protected override void BossDie()
    {
        direction.SetActive(false);
    }
}