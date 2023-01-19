using UnityEngine;
using MyMath;

public class BossRushDir : MonoBehaviour
{
    Vector3 pastRotationVec;
    Vector3 curRotationVec;
    GameObject boss;
    SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        pastRotationVec = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (boss == null) return;
        if (GameManager.IsPaused) return;

        transform.position = boss.transform.position;
        curRotationVec = MyRotation.Rotate(transform.position, Player.playerPos, out bool flip);
        transform.Rotate(curRotationVec - pastRotationVec);
        spriteRenderer.flipY = flip;

        pastRotationVec = curRotationVec;
    }

    public void SetBoss(GameObject boss)
    {
        this.boss = boss;
        transform.position = boss.transform.position;
    }
}