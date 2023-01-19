using UnityEngine;

//아이템을 끌어당기는 범위 설정용
public class ItemGetRange : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.CompareTag(Tags.item) || col.CompareTag(Tags.treasureBox))
        {
            if(col.gameObject.activeSelf)
                col.GetComponent<Item>().Touch();
            return;
        }
    }
}