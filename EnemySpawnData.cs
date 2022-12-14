//적 스폰용 데이터 저장 클래스
public class EnemySpawnData
{
    public readonly int enemyId;
    public readonly float interval;

    public EnemySpawnData(int id, float time)
    {
        enemyId = id;
        interval = time;
    }
}