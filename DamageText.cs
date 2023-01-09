using UnityEngine;

public class DamageText : MonoBehaviour
{
    Vector3 dir;

    void OnEnable()
    {
        dir = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 1).normalized;
    }

    void Update()
    {
        transform.Translate(dir * Time.deltaTime);
    }
}