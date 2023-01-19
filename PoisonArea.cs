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
            col.GetComponent<Player>().OnDamaged(dotDmg, true); //도트데미지
        }
    }

    IEnumerator TimeOver()
    {
        float count = 0;
        while (gameObject.activeSelf)
        {
            if (GameManager.IsPaused) //일시정지 상태인 동안은 대기
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