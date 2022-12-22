using UnityEngine;

public class ExpGem : MonoBehaviour
{
    [SerializeField] int exp;
    [SerializeField] float moveSpeed;
    [SerializeField] float speedOffset;
    float moveDelta;
    bool touched;

    void OnDisable()
    {
        touched = false;
    }

    void Update()
    {
        if (!touched) return;

        moveDelta = moveSpeed * Time.deltaTime + speedOffset;
        transform.position = Vector3.MoveTowards(transform.position, Player.playerPos, moveDelta);
    }

    public void Touch()
    {
        touched = true;
    }

    public int GetExp()
    {
        gameObject.SetActive(false);
        return exp;
    }
}
