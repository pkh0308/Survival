//로비의 플레이어 선택창 갱신용 데이터
public class PlayerInfo
{
    public readonly int charId;
    public readonly string charName;
    public readonly int weaponId;

    public PlayerInfo(int charId, string charName, int weaponId)
    {
        this.charId = charId;
        this.charName = charName;
        this.weaponId = weaponId;
    }
}