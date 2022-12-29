//���� �� �� �������� �����ų ������
public class DataForLevelUp
{
    public readonly int id;
    public readonly int level;
    public readonly string name;
    public readonly string description;

    public DataForLevelUp(WeaponData data)
    {
        id = data.WeaponId;
        name = data.WeaponName;
        description = data.WeaponDescription;
    }

    public DataForLevelUp(AccessoryData data)
    {
        id = data.AccesoryId;
        name = data.AccessoryName;
        description = data.AccessoryDescription;
    }
}