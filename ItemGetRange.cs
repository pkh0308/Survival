using UnityEngine;

public class ItemGetRange : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.CompareTag(Tags.item))
        {
            if(col.gameObject.activeSelf)
                col.GetComponent<Item>().Touch();
            return;
        }
    }
}