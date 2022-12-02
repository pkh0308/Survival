
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
}
