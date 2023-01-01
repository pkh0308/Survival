using UnityEngine;

public class LotteryHighlight : MonoBehaviour
{
    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void OnEnable()
    {
        anim.SetTrigger("DoRotate");
    }
}