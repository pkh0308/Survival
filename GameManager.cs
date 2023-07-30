using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("컴포넌트")]
    [SerializeField] ObjectManager objectManager;
    [SerializeField] UiManager uiManager;
    [SerializeField] GameObject stageEnder;

    //상태 표시용 bool값
    static bool isPaused;
    public static bool IsPaused { get { return isPaused; } }
    bool gameOver;
    bool onLevelUp;
    bool onLottery;

    //타이머 관련
    int seconds;
    Coroutine timerRoutine;

    //적 스폰 관련
    [Header("적 스폰")]
    int curStageIdx;
    Coroutine spawnRoutine;
    List<EnemySpawnData> spawnList;
    int curSpawnIdx;
    Vector3 spawnOffset;
    [SerializeField] float spawnDistance;
    float hMin, hMax, vMin, vMax;
    public static bool hRepos;
    public static bool vRepos;

    //보스 관련
    [Header("보스 스폰")]
    bool onBossFighting;
    public static Action bossDie;
    GameObject area;

    //경험치 관련
    int curExp;
    int[] maxExp;
    int maxExpIdx;

    //아이템 관련
    [Header("아이템")]
    Dictionary<int, int> itemDic;
    [SerializeField] GameObject magnetArea;
    [SerializeField] GameObject bombArea;

    //아이템 박스 스폰 관련
    [Header("아이템 박스")]
    Coroutine spawnItemBoxRoutine;
    Vector3 spawnItemBoxOffset;
    [SerializeField] float spawnItemBoxDistance;
    [SerializeField] float spawnItemBoxTime;

    //레벨업 관련
    [Header("레벨업")]
    [SerializeField] float levelUpInterval;
    Coroutine levelUpRoutine;

    //카운트 관련
    public static Action killCountPlus;
    int killCount;
    public static Action<int> goldCountPlus;
    int goldCount;

    // WaitForSeconds
    float stageClearInterval = 3.0f;
    float gameOverInterval = 1.5f;
    float onOffInterval = 0.1f;
    float oneSecond = 1.0f;

    void Awake()
    {
        seconds = 0;

        isPaused = false;
        onLevelUp = false;
        onLottery = false;
        onBossFighting = false;

        maxExp = Enumerable.Repeat<int>(100, 100).ToArray();

        itemDic = new Dictionary<int, int>();

        killCountPlus = () => { UpdateKillCount(); };
        goldCountPlus = (a) => { UpdateGoldCount(a); };

        spawnList = new List<EnemySpawnData>();
        curStageIdx = LoadingSceneManager.Inst.CurStageIdx;
        curSpawnIdx = 0;
        bossDie = () => { BossDie(); };

        ReadSpawnData();
        ReadItemData();
        ReadStageData();
    }

    void ReadSpawnData()
    {
        //csv 파일에서 데이터 읽어오기
        TextAsset spawnDatas = Resources.Load("SpawnDatas") as TextAsset;
        StringReader spawnDataReader = new StringReader(spawnDatas.text);

        if (spawnDataReader == null)
        {
            Debug.Log("spawnDataReader is null");
            return;
        }
        //첫줄 스킵(변수 이름 라인)
        string line = spawnDataReader.ReadLine();
        if (line == null) return;

        line = spawnDataReader.ReadLine();
        while (line.Length > 1)
        {
            string[] datas = line.Split(',');
            if (int.Parse(datas[0]) < curStageIdx)
            {
                line = spawnDataReader.ReadLine();
                continue;
            }
            if (int.Parse(datas[0]) > curStageIdx)
                break;

            //1: id, 2: interval
            spawnList.Add(new EnemySpawnData(int.Parse(datas[1]), float.Parse(datas[2])));

            line = spawnDataReader.ReadLine();
            if (line == null) break;
        }
        spawnDataReader.Close();
    }

    void ReadItemData()
    {
        //csv 파일에서 데이터 읽어오기
        TextAsset itemDatas = Resources.Load("ItemDatas") as TextAsset;
        StringReader itemDataReader = new StringReader(itemDatas.text);

        if (itemDataReader == null)
        {
            Debug.Log("itemDataReader is null");
            return;
        }
        //첫줄 스킵(변수 이름 라인)
        string line = itemDataReader.ReadLine();
        if (line == null) return;

        line = itemDataReader.ReadLine();
        while (line.Length > 1)
        {
            string[] datas = line.Split(',');
            itemDic.Add(int.Parse(datas[0]), int.Parse(datas[1]));

            line = itemDataReader.ReadLine();
            if (line == null) break;
        }
        itemDataReader.Close();
    }

    void ReadStageData()
    {
        //csv 파일에서 데이터 읽어오기
        TextAsset stageDatas = Resources.Load("StageDatas") as TextAsset;
        StringReader stageDataReader = new StringReader(stageDatas.text);

        if (stageDataReader == null)
        {
            Debug.Log("stageDataReader is null");
            return;
        }
        //첫줄 스킵(변수 이름 라인)
        string line = stageDataReader.ReadLine();
        if (line == null) return;

        line = stageDataReader.ReadLine();
        while (line.Length > 1)
        {
            string[] datas = line.Split(',');
            if (datas[0] != LoadingSceneManager.Inst.CurStageIdx.ToString())
            {
                line = stageDataReader.ReadLine();
                continue;
            }

            // 5: hMin, 6: hMax, 7: vMin, 8: vMax
            hMin = float.Parse(datas[5]);
            hMax = float.Parse(datas[6]);
            vMin = float.Parse(datas[7]);
            vMax = float.Parse(datas[8]);
            break;
        }
        stageDataReader.Close();

        //타입 설정
        hRepos = hMax < 1000 ? false : true;
        vRepos = vMax < 1000 ? false : true;
    }

    void Start()
    {
        timerRoutine = StartCoroutine(Timer());
        spawnRoutine = StartCoroutine(Spawn());
        spawnItemBoxRoutine = StartCoroutine(SpawnItemBox());
    }

    //타이머 관련
    IEnumerator Timer()
    {
        while (isPaused == false)
        {
            if (gameOver) yield break;

            yield return WfsManager.Instance.GetWaitForSeconds(oneSecond);
            seconds++;
            uiManager.UpdateTimer(seconds);
        }
    }

    public void Pause_Board()
    {
        if (onLevelUp || onLottery || gameOver) return;

        if(uiManager.Pause(isPaused))
            Pause();
        else
            PauseOff();
    }

    public void Pause()
    {
        if(isPaused) return;

        isPaused = true;
        StopMyCoroutines();
    }

    public void PauseOff()
    {
        if (!isPaused) return;

        isPaused = false;
        StartMyCoroutines();
        Weapons.restartWeapons();
    }

    void StartMyCoroutines()
    {
        timerRoutine = StartCoroutine(Timer());
        spawnItemBoxRoutine = StartCoroutine(SpawnItemBox());

        if (onLevelUp) LevelUp();
    }

    void StopMyCoroutines()
    {
        if (timerRoutine != null) StopCoroutine(timerRoutine);
        if (spawnItemBoxRoutine != null) StopCoroutine(spawnItemBoxRoutine);

        if (onLevelUp) StopCoroutine(levelUpRoutine);
    }

    //적 스폰 관련
    IEnumerator Spawn()
    {
        GameObject curEnemy;
        float count;
        while (curSpawnIdx < spawnList.Count)
        {
            EnemySpawnData curData = spawnList[curSpawnIdx];
            count = 0;
            while(count < curData.interval)
            {
                if (isPaused)
                {
                    yield return null;
                    continue;
                }

                count += Time.deltaTime;
                if (count >= curData.interval) break;
                yield return null;
            }

            if(curData.enemyId == ObjectNames.bossAlert)
            {
                StartCoroutine(BossReady(curData.interval));
            }
            else if(curData.enemyId != 0)
            {
                curEnemy = objectManager.MakeEnemy(curData.enemyId);
                //랜덤 벡터값 생성
                spawnOffset = Quaternion.Euler(0, 0, UnityEngine.Random.Range(0, 360)) * Vector3.right * spawnDistance + Player.playerPos;
                //x값이나 y값이 경계 바깥일 경우 중간값으로 조정
                if (spawnOffset.x != Mathf.Clamp(spawnOffset.x, hMin, hMax))
                    spawnOffset.x = (hMin + hMax) / 2;
                if (spawnOffset.y != Mathf.Clamp(spawnOffset.y, vMin, vMax))
                    spawnOffset.y = (vMin + vMax) / 2;
                curEnemy.transform.position = spawnOffset;
                if(curData.enemyId >= ObjectNames.monsterTree) //보스일 경우
                {
                    area = objectManager.MakeObj(ObjectNames.bossArea);
                    area.transform.position = curEnemy.transform.position;
                }
            }
            curSpawnIdx++;
        }
    }

    IEnumerator BossReady(float interval)
    {
        StartCoroutine(uiManager.BossAlert(interval - 0.5f));
        float count = 0;
        while(count < interval - 0.5f) 
        {
            if(isPaused)
            {
                yield return null;
                continue;
            }
            count += Time.deltaTime;
            yield return null;
        }

        stageEnder.SetActive(true);
        onBossFighting = true;
        count = 0;
        while (count < 0.4f)
        {
            if (isPaused)
            {
                yield return null;
                continue;
            }
            count += Time.deltaTime;
            yield return null;
        }
        stageEnder.SetActive(false);
        uiManager.ChangeProgressBar();
    }

    //아이템 박스 스폰
    IEnumerator SpawnItemBox()
    {
        // 초기 쿨타임
        yield return WfsManager.Instance.GetWaitForSeconds(spawnItemBoxTime);

        GameObject itemBox;
        while(!isPaused)
        {
            if (onBossFighting) yield break;

            itemBox = objectManager.MakeObj(ObjectNames.itemBox);
            //랜덤 벡터값 생성
            spawnItemBoxOffset = Quaternion.Euler(0, 0, UnityEngine.Random.Range(0, 360)) * Vector3.right * spawnItemBoxDistance + Player.playerPos; 
            //x값이나 y값이 경계 바깥일 경우 중간값으로 조정
            if (spawnItemBoxOffset.x != Mathf.Clamp(spawnItemBoxOffset.x, hMin, hMax))
                spawnItemBoxOffset.x = (hMin + hMax) / 2;
            if (spawnItemBoxOffset.y != Mathf.Clamp(spawnItemBoxOffset.y, vMin, vMax))
                spawnItemBoxOffset.y = (vMin + vMax) / 2;
            itemBox.transform.position = spawnOffset;

            yield return WfsManager.Instance.GetWaitForSeconds(spawnItemBoxTime);
        }
    }

    //아이템 소비
    public void GetItem(int id)
    {
        //이미 소모된 아이템 재습득 방지
        if (id == -1) return;

        switch(id)
        {
            case ObjectNames.exp_10:
            case ObjectNames.exp_50:
            case ObjectNames.exp_250:
            {
                SoundManager.playSfx(SoundManager.Sfx.getExp);
                curExp += itemDic[id];
                //대기중일 경우 레벨업 루틴은 스킵
                if (!onLevelUp && maxExp[maxExpIdx] <= curExp)
                    levelUpRoutine = StartCoroutine(LevelUpRoutine());
                uiManager.UpdateExp(curExp, maxExp[maxExpIdx]);
                break;
            }
            case ObjectNames.meat_50:
            {
                SoundManager.playSfx(SoundManager.Sfx.meat_or_magnet);
                Player.getHeal(itemDic[id]);
                break;
            }
            case ObjectNames.gold_70:
            case ObjectNames.gold_10:
            case ObjectNames.gold_50:
            case ObjectNames.gold_100:
            {
                SoundManager.playSfx(SoundManager.Sfx.gold);
                goldCount += itemDic[id];
                GoldManager.Instance.PlusGold(itemDic[id]);
                uiManager.UpdateGoldCount(goldCount);
                break;
            }
            case ObjectNames.magnet:
            {
                SoundManager.playSfx(SoundManager.Sfx.meat_or_magnet);
                StartCoroutine(AreaOnOff(magnetArea));
                break;
            }
            case ObjectNames.bomb:
            {
                SoundManager.playSfx(SoundManager.Sfx.bomb);
                StartCoroutine(AreaOnOff(bombArea));
                break;
            }
            case ObjectNames.treasureBox:
            {
                GetTreasureBox();
                break;
            }  
        }
    }

    IEnumerator AreaOnOff(GameObject obj)
    {
        obj.SetActive(true);
        yield return WfsManager.Instance.GetWaitForSeconds(onOffInterval);
        obj.SetActive(false);
    }

    //보물 상자 획득
    public void GetTreasureBox()
    {
        if (gameOver) return;

        Pause();
        onLottery = true;
        if (onLevelUp) //레벨업 대기중일 경우 레벨업 루틴 정지
            StopCoroutine(levelUpRoutine);
        uiManager.ShowLottery();
    }

    public void EndTreasureBox(int lotteryMoney)
    {
        UpdateGoldCount(lotteryMoney);
        onLottery = false;

        if (onLevelUp) //레벨업 대기중일 경우 레벨업 루틴 재시작
        {
            levelUpRoutine = StartCoroutine(LevelUpRoutine());
        }
    }

    //LevelUp 루틴 외부 호출용
    public void LevelUp()
    {
        if (isPaused || gameOver) return; 
        if (maxExp[maxExpIdx] > curExp)
        {
            onLevelUp = false;
            return;
        }
        
        levelUpRoutine = StartCoroutine(LevelUpRoutine());
    }
    
    IEnumerator LevelUpRoutine()
    {
        onLevelUp = true; 
        curExp -= maxExp[maxExpIdx]; 
        maxExpIdx++;
        SoundManager.playSfx(SoundManager.Sfx.levelUp);
        yield return WfsManager.Instance.GetWaitForSeconds(levelUpInterval);

        if (gameOver) yield break;
        Pause();
        uiManager.UpdateLevel(maxExpIdx + 1);
        uiManager.UpdateExp(curExp, maxExp[maxExpIdx]);
        uiManager.WeaponSelect();
    }

    //카운트 관련
    void UpdateKillCount()
    {
        killCount++;
        uiManager.UpdateKillCount(killCount);
    }

    void UpdateGoldCount(int count)
    {
        goldCount += count;
        uiManager.UpdateGoldCount(goldCount);
    }

    public void BossDie()
    {
        area.SetActive(false);
        uiManager.ChangeProgressBar();
        StartCoroutine(StageClear());
    }

    //스테이지 클리어
    IEnumerator StageClear()
    {
        gameOver = true;

        yield return WfsManager.Instance.GetWaitForSeconds(stageClearInterval);
        isPaused = true;
        uiManager.StageClear(killCount, goldCount);
    }

    //게임 오버
    public void PlayerDie()
    {
        StartCoroutine(GameOver());
    }

    IEnumerator GameOver()
    {
        gameOver = true;

        yield return WfsManager.Instance.GetWaitForSeconds(gameOverInterval);
        isPaused = true;
        uiManager.GameOver(killCount, goldCount);
    }
}