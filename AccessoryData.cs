//�Ǽ��縮 ������ csv ���Ͽ��� �о���� �� �����ϱ� ���� Ŭ����
public class AccessoryData
{
    //���̵�
    int accesoryId;
    public int AccesoryId { get { return accesoryId; } }
    //����
    int accessoryLevel;
    public int AccessoryLevel { get { return accessoryLevel; } }
    //�̸�
    string accessoryName;
    public string AccessoryName { get { return accessoryName; } }
    //��
    float accessoryValue;
    public float AccessoryValue { get { return accessoryValue; } }
    //����
    string accessoryDescription;
    public string AccessoryDescription { get { return accessoryDescription; } }

    public AccessoryData(int id, int lv, string name, float val, string desc)
    {
        accesoryId = id;
        accessoryLevel = lv;
        accessoryName = name;
        accessoryValue = val;
        accessoryDescription = desc;
    }
}