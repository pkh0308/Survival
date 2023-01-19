using System.Collections;
using UnityEngine;

public class PoisonArea : MonoBehaviour
{
    [SerializeField] float timeOutSec;
    [SerializeField] int dotDmg;

    void OnEnable()
    {
        StartCoroutine(TimeOver());
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (GameManager.IsPaused) return;

        if(col.CompareTag(Tags.player))
        {
            col.GetComponent<Player>().OnDamaged(dotDmg, true); //��Ʈ������
        }
    }

    IEnumerator TimeOver()
    {
        float count = 0;
        while (gameObject.activeSelf)
        {
            if (GameManager.IsPaused) //�Ͻ����� ������ ������ ���
            {
                yield return null;
                continue;
            }
            count += Time.deltaTime;
            if (count > timeOutSec)
                gameObject.SetActive(false);
            
            yield return null;
        }
    }
}