//���� ���׷��̵�� ������ Ŭ����
public class WeaponUpgradeData
{
    public readonly int weaponId;
    public readonly int baseWeaponId;
    public readonly int combineId;

    public WeaponUpgradeData(int weaponId, int baseWeaponId, int combineId)
    {
        this.weaponId = weaponId;
        this.baseWeaponId = baseWeaponId;
        this.combineId = combineId;
    }
}