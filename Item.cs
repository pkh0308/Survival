using System.Collections;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] protected int id;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float speedOffset;
    protected float moveDelta;

    public void Touch()
    {
        StartCoroutine(nameof(Trace));
    }

    IEnumerator Trace()
    {
        while (gameObject.activeSelf)
        {
            moveDelta = moveSpeed * Time.deltaTime + speedOffset;
            transform.position = Vector3.MoveTowards(transform.position, Player.playerPos, moveDelta);
            yield return null;
        }
    }

    public int Consume()
    {
        StopCoroutine(nameof(Trace));
        gameObject.SetActive(false);
        return id;
    }

    //ÀÚ¼® È¹µæ ½Ã Ã³¸®
    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.CompareTag(Tags.magnet))
            StartCoroutine(nameof(Trace));
    }
}
