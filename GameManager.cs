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
    [SerializeField] StageSoundManager soundManager;
    [SerializeField] GameObject stageEnder;

    static bool isPaused;
    public static bool IsPaused { get { return isPaused; } }
    bool gameOver;

    //타이머 관련
    WaitForSeconds oneSec;
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
    [SerializeField] float maxStageTime;

    //경험치 관련
    int curExp;
    int[] maxExp;
    int maxExpIdx;

    //아이템 관련
    [Header("아이템")]
    Dictionary<int, int> itemDic;
    [SerializeField] GameObject magnetArea;
    [SerializeField] GameObject bombArea;
    WaitForSeconds onoffInterval;

    //아이템 박스 스폰 관련
    [Header("아이템 박스")]
    Coroutine spawnItemBoxRoutine;
    WaitForSeconds spawnItemBoxInterval;
    Vector3 spawnItemBoxOffset;
    [SerializeField] float spawnItemBoxDistance;
    [SerializeField] float spawnItemBoxTime;

    //레벨업 관련
    [Header("레벨업")]
    [SerializeField] float levelUpInterval;
    WaitForSeconds levelUpSeconds;
    Coroutine levelUpRoutine;
    bool isWaitingForLevelUp;

    //카운트 관련
    public static Action killCountPlus;
    int killCount;
    public static Action<int> moneyCountPlus;
    int moneyCount;

    void Awake()
    {
        oneSec = new WaitForSeconds(1.0f);
        seconds = 0;

        levelUpSeconds = new WaitForSeconds(levelUpInterval);

        isPaused = false;
        isWaitingForLevelUp = false;

        maxExp = Enumerable.Repeat<int>(100, 100).ToArray();

        itemDic = new Dictionary<int, int>();
        onoffInterval = new WaitForSeconds(0.1f);

        killCountPlus = () => { UpdateKillCount(); };
        moneyCountPlus = (a) => { UpdateMoneyCount(a); };

        spawnList = new List<EnemySpawnData>();
        curStageIdx = LoadingSceneManager.getCurStageIdx();
        curSpawnIdx = 0;

        spawnItemBoxInterval = new WaitForSeconds(spawnItemBoxTime);

        ReadSpawnData();
        ReadItemData();
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

    void Start()
    {
        StartMyCoroutines();
    }

    //타이머 관련
    IEnumerator Timer()
    {
        while (isPaused == false)
        {
            yield return oneSec;
            seconds++;
            uiManager.UpdateTimer(seconds);

            if (seconds == maxStageTime) StartCoroutine(StageClear());
        }
    }

    public void Pause_Exit()
    {
        isPaused = uiManager.Pause(isPaused);
        if (isPaused)
        {
            StopMyCoroutines();
            Time.timeScale = 0;
        }
        else
        {
            StartMyCoroutines();
            Time.timeScale = 1;
        }
    }

    public void Pause()
    {
        if(isPaused) return;

        isPaused = true;
        StopMyCoroutines();
        Time.timeScale = 0;
    }

    public void PauseOff()
    {
        if (!isPaused) return;

        isPaused = false;
        StartMyCoroutines();
        Time.timeScale = 1;
    }

    void StartMyCoroutines()
    {
        timerRoutine = StartCoroutine(Timer());
        spawnRoutine = StartCoroutine(Spawn());
        spawnItemBoxRoutine = StartCoroutine(SpawnItemBox());
    }

    void StopMyCoroutines()
    {
        if (timerRoutine != null) StopCoroutine(timerRoutine);
        if (spawnRoutine != null) StopCoroutine(spawnRoutine);
        if (spawnItemBoxRoutine != null) StopCoroutine(spawnItemBoxRoutine);
    }

    //적 스폰 관련
    IEnumerator Spawn()
    {
        GameObject curEnemy;
        while (!isPaused)
        {
            if (curSpawnIdx >= spawnList.Count) break;

            EnemySpawnData curData = spawnList[curSpawnIdx];
            if (curData.enemyId != 0)
            {
                curEnemy = objectManager.MakeObj(curData.enemyId);
                spawnOffset = Quaternion.Euler(0, 0, UnityEngine.Random.Range(0, 360)) * Vector3.right * spawnDistance; //랜덤 벡터값 생성
                curEnemy.transform.position = Player.playerPos + spawnOffset; // 플레이어로부터 일정 거리 떨어진 곳에서 스폰
            }
            curSpawnIdx++;

            yield return new WaitForSeconds(curData.interval);
        }
    }

    //아이템 박스 스폰
    IEnumerator SpawnItemBox()
    {
        yield return spawnItemBoxInterval;

        GameObject itemBox;
        while(!isPaused)
        {
            itemBox = objectManager.MakeObj(ObjectNames.itemBox);
            spawnItemBoxOffset = Quaternion.Euler(0, 0, UnityEngine.Random.Range(0, 360)) * Vector3.right * spawnItemBoxDistance; //랜덤 벡터값 생성
            itemBox.transform.position = Player.playerPos + spawnItemBoxOffset; // 플레이어로부터 일정 거리 떨어진 곳에서 스폰

            yield return spawnItemBoxInterval;
        }
    }

    //아이템 소비
    public void GetItem(int id)
    {
        switch(id)
        {
            case ObjectNames.exp_10:
            case ObjectNames.exp_50:
            case ObjectNames.exp_250:
                {
                    soundManager.PlaySfx((int)StageSoundManager.StageSfx.getExp);
                    curExp += itemDic[id];

                    if (maxExp[maxExpIdx] <= curExp)
                        levelUpRoutine = StartCoroutine(LevelUpRoutine());
                    uiManager.UpdateExp(curExp, maxExp[maxExpIdx]);

                    break;
                }
            case ObjectNames.meat_50:
                {
                    soundManager.PlaySfx((int)StageSoundManager.StageSfx.meat_or_magnet);
                    Player.getHeal(itemDic[id]);
                    break;
                }
            case ObjectNames.gold_70:
            case ObjectNames.gold_10:
            case ObjectNames.gold_50:
            case ObjectNames.gold_100:
                {
                    soundManager.PlaySfx((int)StageSoundManager.StageSfx.gold);
                    GoldManager.Instance.PlusGold(itemDic[id]);
                    uiManager.UpdateMoneyCount(GoldManager.Instance.Gold);
                    break;
                }
            case ObjectNames.magnet:
                {
                    soundManager.PlaySfx((int)StageSoundManager.StageSfx.meat_or_magnet);
                    StartCoroutine(AreaOnOff(magnetArea));
                    break;
                }
            case ObjectNames.bomb:
                {
                    soundManager.PlaySfx((int)StageSoundManager.StageSfx.bomb);
                    StartCoroutine(AreaOnOff(bombArea));
                    break;
                }
        }
    }

    IEnumerator AreaOnOff(GameObject obj)
    {
        obj.SetActive(true);
        yield return onoffInterval;
        obj.SetActive(false);
    }

    //LevelUp 루틴 외부 호출용
    public void LevelUp()
    {
        if (isPaused || gameOver) return;
        if (maxExp[maxExpIdx] > curExp) return;
        if (isWaitingForLevelUp) return;

        levelUpRoutine = StartCoroutine(LevelUpRoutine());
    }
    
    IEnumerator LevelUpRoutine()
    {
        isWaitingForLevelUp = true;
        soundManager.PlaySfx((int)StageSoundManager.StageSfx.levelUp);
        curExp -= maxExp[maxExpIdx];
        maxExpIdx++;

        yield return levelUpSeconds;
        Pause();
        uiManager.UpdateLevel(maxExpIdx + 1);
        uiManager.UpdateExp(curExp, maxExp[maxExpIdx]);
        uiManager.WeaponSelect();
        isWaitingForLevelUp = false;
    }

    //카운트 관련
    void UpdateKillCount()
    {
        killCount++;
        uiManager.UpdateKillCount(killCount);
    }

    void UpdateMoneyCount(int count)
    {
        moneyCount += count;
        uiManager.UpdateMoneyCount(moneyCount);
    }

    //스테이지 클리어
    IEnumerator StageClear()
    {
        gameOver = true;
        //모든 적 사망처리
        stageEnder.SetActive(true);

        yield return new WaitForSeconds(3.0f);
        isPaused = true;
        uiManager.StageClear(killCount, moneyCount);
    }

    //게임 오버
    public void GameOver()
    {
        isPaused = true;
        gameOver = true;
        uiManager.GameOver(killCount, moneyCount);
    }
}
