using UnityEngine;

//카메라 위치 갱신용 클래스
//플레이어를 따라다니도록 설정
public class PlayerCamera : MonoBehaviour
{
    Vector3 cameraPos;

    private void Awake()
    {
        cameraPos = new Vector3(0, 0, -10);
    }

    void Update()
    {
        transform.position = Player.playerPos + cameraPos;
    }
}
