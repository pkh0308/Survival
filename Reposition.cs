using UnityEngine;

public class Reposition : MonoBehaviour
{
    [SerializeField] float posVal;
    Collider2D col;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        if (!coll.CompareTag(Tags.area)) return;
        
        float diff_x = Player.playerPos.x - transform.position.x; 
        float diff_y = Player.playerPos.y - transform.position.y;

        float abs_x = Mathf.Abs(diff_x);
        float abs_y = Mathf.Abs(diff_y);

        diff_x = diff_x > 0 ? 1 : -1;
        diff_y = diff_y > 0 ? 1 : -1;
        
        switch (transform.tag)
        {
            case Tags.ground:
                if(GameManager.hRepos && GameManager.vRepos)
                {
                    if (abs_x > abs_y)
                        transform.Translate(Vector3.right * diff_x * posVal);
                    else if (abs_x < abs_y)
                        transform.Translate(Vector3.up * diff_y * posVal);
                }
                else if(GameManager.hRepos) //수평 이동만 가능
                {
                    transform.Translate(Vector3.right * diff_x * posVal);
                }
                else if(GameManager.vRepos) //수직 이동만 가능
                {
                    transform.Translate(Vector3.up * diff_y * posVal);
                }
                break;
            case Tags.enemy:
                if (!col.enabled) return;

                if (abs_x > abs_y && GameManager.hRepos)
                {
                    transform.Translate(Vector3.right * diff_x * posVal);
                }
                else if (abs_x < abs_y && GameManager.vRepos)
                {
                    transform.Translate(Vector3.up * diff_y * posVal);
                }
                break;
        }
    }
}