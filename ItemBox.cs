using UnityEngine;
using System.Collections;

public class ItemBox : MonoBehaviour
{
    Animator anim;
    WaitForSeconds openSec;
    BoxCollider2D coll;

    void Awake()
    {
        anim = GetComponent<Animator>();
        coll = GetComponent<BoxCollider2D>();
        openSec = new WaitForSeconds(1.0f);
    }

    void OnEnable()
    {
        coll.enabled = true;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.CompareTag(Tags.playerBullet))
            StartCoroutine(DropItem());
    }

    IEnumerator DropItem()
    {
        coll.enabled = false;
        anim.SetTrigger("OpenBox");

        int val = Random.Range(0, 10000);
        GameObject item = null;
        
        if (val > 9500)      item = ObjectManager.makeItem(ObjectNames.bomb);
        else if (val > 9000) item = ObjectManager.makeItem(ObjectNames.magnet);
        else if (val > 8500) item = ObjectManager.makeItem(ObjectNames.gold_100);
        else if (val > 7500) item = ObjectManager.makeItem(ObjectNames.gold_50);
        else if (val > 6500) item = ObjectManager.makeItem(ObjectNames.meat_50);
        else if (val > 5000) item = ObjectManager.makeItem(ObjectNames.gold_10);
        if (item != null) item.transform.position = transform.position;

        yield return openSec;
        gameObject.SetActive(false);
    }
}