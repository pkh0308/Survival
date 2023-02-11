using UnityEngine;

//������Ʈ ȸ���� �Լ� ���� ���ӽ����̽�
//ȸ�� ������ �ʿ��� Ŭ�������� �����Ͽ� ���
namespace MyMath
{
    public class MyRotation
    {
        //���⺤�͸� ���� ȸ����Ű�� ���
        public static Vector3 Rotate(Vector3 dir)
        {
            if (dir.y == 0) dir.y = 0.01f; // DevideByZero ����

            float angle = Mathf.Atan(dir.x / dir.y) * Mathf.Rad2Deg * -1;
            //�Ʒ� ������ ���
            if (dir.y < 0) angle += -180;
            Vector3 rotationVec = Vector3.forward * angle;
            return rotationVec;
        }

        //�߻��ڿ� Ÿ���� ��ġ���� ���� ȸ����Ű�� �Լ�
        public static Vector3 Rotate(Vector3 pos, Vector3 target)
        {
            float diff_x = target.x - pos.x;
            float diff_y = target.y - pos.y;
            if (diff_y == 0) diff_y = 0.01f; // DevideByZero ����

            float angle = Mathf.Atan(diff_x / diff_y) * Mathf.Rad2Deg * -1;
            //Ÿ���� y��ǥ�� �÷��̾�� �Ʒ����� ���
            if (diff_y < 0) angle += -180;
            Vector3 rotationVec = Vector3.forward * angle;
            return rotationVec;
        }
    }
}