using UnityEngine;

public class ExpGetRange : MonoBehaviour
{
    [SerializeField] GameManager gameManager;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag(Tags.expGem))
        {
            col.GetComponent<ExpGem>().Touch();
            return;
        }
    }
}
