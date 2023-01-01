using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour
{
    int dmg;
    int id;
    Collider2D col;

    void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    //Ȱ��ȭ �� �ڵ����� ��Ȱ��ȭ
    void OnEnable()
    {
        col.enabled = true;
        StartCoroutine(Exit());
    }

    void OnDisable()
    {
        transform.localScale = Vector3.one;
    }

    IEnumerator Exit()
    {
        yield return new WaitForSeconds(0.1f);
        col.enabled = false;

        yield return new WaitForSeconds(0.4f);
        gameObject.SetActive(false);
    }

    public void Initialize(int dmg, int id, float scale)
    {
        this.dmg = dmg;
        this.id = id;
        transform.localScale = Vector3.one * scale;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag(Tags.enemy)) return;

        col.GetComponent<Enemy>().OnDamaged(dmg);
        Weapons.accumulateDmg(id, dmg);
    }
}
