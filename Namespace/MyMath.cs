using UnityEngine;

//오브젝트 회전용 함수 선언 네임스페이스
//회전 로직이 필요한 클래스에서 참조하여 사용
namespace MyMath
{
    public class MyRotation
    {
        //방향벡터를 통해 회전시키는 경우
        public static Vector3 Rotate(Vector3 dir)
        {
            if (dir.y == 0) dir.y = 0.01f; // DevideByZero 방지

            float angle = Mathf.Atan(dir.x / dir.y) * Mathf.Rad2Deg * -1;
            //아래 방향일 경우
            if (dir.y < 0) angle += -180;
            Vector3 rotationVec = Vector3.forward * angle;
            return rotationVec;
        }

        //발사자와 타겟의 위치값을 통해 회전시키는 함수
        public static Vector3 Rotate(Vector3 pos, Vector3 target)
        {
            float diff_x = target.x - pos.x;
            float diff_y = target.y - pos.y;
            if (diff_y == 0) diff_y = 0.01f; // DevideByZero 방지

            float angle = Mathf.Atan(diff_x / diff_y) * Mathf.Rad2Deg * -1;
            //타겟의 y좌표가 플레이어보다 아래쪽일 경우
            if (diff_y < 0) angle += -180;
            Vector3 rotationVec = Vector3.forward * angle;
            return rotationVec;
        }
    }
}