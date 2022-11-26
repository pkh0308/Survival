//로비의 강화 화면용 데이터 클래스
//LobbyManager에서 csv 파일을 읽어들인 뒤 사용
public class EnhancementData
{
    public enum enhanceType {
        atkPower = 1, atkScale, projSpeed, coolTime, projCount, atkRemainTime, playerHealth, playerdef, playeMoveSpeed
    }

    public readonly enhanceType type;
    public readonly float enhanceVal;
    public readonly int enhanceId;
    public readonly int spriteId;
    public readonly string nameText;
    public readonly string descText;

    public EnhancementData(int id, int s_id, string name, string desc, int t, float val)
    {
        type = (enhanceType)t;
        enhanceId = id;
        spriteId = s_id;
        nameText = name;
        descText = desc;
        enhanceVal = val;
    }
}