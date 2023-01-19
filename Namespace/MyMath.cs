using UnityEngine;

//������Ʈ ȸ���� �Լ� ���� ���ӽ����̽�
//ȸ�� ������ �ʿ��� Ŭ�������� �����Ͽ� ���
namespace MyMath
{
    public class MyRotation
    {
        //���⺤�͸� ���� ȸ����Ű�� ���
        public static Vector3 Rotate(Vector3 dir, out bool flipY)
        {
            if (dir.x == 0) dir.x = 0.01f; // DevideByZero ����

            float angle = Mathf.Atan(dir.x / dir.y) * Mathf.Rad2Deg * -1;
            Vector3 rotationVec = Vector3.forward * angle;
            //�Ʒ��� �߻��� ���
            flipY = dir.y < 0 ? true : false;
            return rotationVec;
        }

        //�߻��ڿ� Ÿ���� ��ġ���� ���� ȸ����Ű�� �Լ�
        public static Vector3 Rotate(Vector3 pos, Vector3 target, out bool flipY)
        {
            float diff_x = target.x - pos.x;
            float diff_y = target.y - pos.y;
            if (diff_x == 0) diff_x = 0.01f; // DevideByZero ����

            float angle = Mathf.Atan(diff_x / diff_y) * Mathf.Rad2Deg * -1;
            Vector3 rotationVec = Vector3.forward * angle;
            //Ÿ���� y��ǥ�� �÷��̾�� �Ʒ����� ��� ������
            flipY = diff_y < 0 ? true : false;
            return rotationVec;
        }
    }
}