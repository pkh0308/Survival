//스테이지 씬에서 플레이어 위치 추적용
using UnityEngine;

public class TileSearchArea : MonoBehaviour
{
    void Update()
    {
        transform.position = Player.playerPos;
    }
}
