
public class CharacterData
{
    public readonly int characterId; 
    public readonly float atkPower;
    public readonly float atkScale;
    public readonly float projSpeed;
    public readonly float coolTime;
    public readonly int projCount;
    public readonly float atkRemainTime;
    public readonly int playerHealth;
    public readonly int playerdef;
    public readonly float playerMoveSpeed;

    public CharacterData(int id, float power, float scale, float projSpd, float cool, int count, float remain, int hp, int def, float moveSpd)
    {
        characterId = id;
        atkPower = power;
        atkScale = scale;
        projSpeed = projSpd;
        coolTime = cool;
        projCount = count;
        atkRemainTime = remain;
        playerHealth = hp;
        playerdef = def;
        playerMoveSpeed = moveSpd;
    }
}
