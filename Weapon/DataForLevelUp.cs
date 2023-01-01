//레벨 업 시 선택지에 노출시킬 데이터
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
        level = data.WeaponLevel;
        description = data.WeaponDescription;
    }

    public DataForLevelUp(AccessoryData data)
    {
        id = data.AccesoryId;
        name = data.AccessoryName;
        level = data.AccessoryLevel;
        description = data.AccessoryDescription;
    }
}