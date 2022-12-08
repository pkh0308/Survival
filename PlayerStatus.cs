//캐릭터 스테이터스 저장용 클래스
public class PlayerStatus
{
    float atkPowerVal;
    public float AtkPowerVal { get { return atkPowerVal; } }
    float atkScaleVal;
    public float AtkScaleVal { get { return atkScaleVal; } }
    float projSpeedVal;
    public float ProjSpeedVal { get { return projSpeedVal; } }
    float coolTimeVal;
    public float CoolTimeVal { get { return coolTimeVal; } }
    int projCountVal;
    public int ProjCountVal { get { return projCountVal; } }
    float atkRemainTimeVal;
    public float AtkRemainTimeVal { get { return atkRemainTimeVal; } }
    float playerHealthVal;
    public float PlayerHealthVal { get { return playerHealthVal; } }
    int playerdefVal;
    public int PlayerdefVal { get { return playerdefVal; } }
    float playerMoveSpeedVal;
    public float PlayerMoveSpeedVal { get { return playerMoveSpeedVal; } }

    public PlayerStatus(float power, float scale, float projSpeed, float cooltime, int count, float remain, float health, int def, float moveSpeed)
    {
        atkPowerVal = power;
        atkScaleVal = scale;
        projSpeedVal = projSpeed;
        coolTimeVal = cooltime;
        projCountVal = count;
        atkRemainTimeVal = remain;
        playerHealthVal = health;
        playerdefVal = def;
        playerMoveSpeedVal = moveSpeed;
    }

    public void AddStatus(string name, float val)
    {
        switch(name)
        {
            case nameof(atkPowerVal):
                atkPowerVal += val;
                break;
            case nameof(atkScaleVal):
                atkScaleVal += val;
                break;
            case nameof(projSpeedVal):
                projSpeedVal += val;
                break;
            case nameof(coolTimeVal):
                coolTimeVal += val;
                break;
            case nameof(atkRemainTimeVal):
                atkRemainTimeVal += val;
                break;
        }
    }

    public void AddStatus(string name, int val)
    {
        switch (name)
        {
            case nameof(projCountVal):
                projCountVal += val;
                break;
            case nameof(playerdefVal):
                playerdefVal += val;
                break;
        }
    }
}
