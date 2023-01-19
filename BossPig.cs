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
        velocityCutTime = rushEndInterval; //���� ���ӽð��� ������ rigid.velocity �ʱ�ȭ
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
            if (state == State.None) //���� ���� ��
            {
                if(count > rushInterval)
                {
                    count = 0;
                    state = State.Warning;
                    direction.SetActive(true);
                }
            }
            else if(state == State.Warning) //���� ���� ��� ��
            {
                if(count > rushWarningInterval)
                {
                    count = 0;
                    state = State.OnRush;
                    direction.SetActive(false);
                    beforeSpeed = moveSpeed;
                    moveSpeed = 0; //��� �̵� ����
                    rigid.AddForce((Player.playerPos - transform.position).normalized * rushSpeed, ForceMode2D.Impulse);
                }
            }
            else //���� ��
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