using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
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
    int curStageIdx;
    Coroutine spawnRoutine;
    List<EnemySpawnData> spawnList;
    int curSpawnIdx;
    Vector3 spawnOffset;
    [SerializeField] float spawnDistance;

    //경험치 관련
    int curExp;
    int[] maxExp;
    int maxExpIdx;

    //레벨업 관련
    [SerializeField] float levelUpInterval;
    WaitForSeconds levelUpSeconds;
    Coroutine levelUpRoutine;

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

        maxExp = Enumerable.Repeat<int>(100, 100).ToArray();

        killCountPlus = () => { UpdateKillCount(); };
        moneyCountPlus = (a) => { UpdateMoneyCount(a); };

        spawnList = new List<EnemySpawnData>();
        curStageIdx = LoadingSceneManager.getCurStageIdx();
        curSpawnIdx = 0;
        ReadSpawnData();
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

    void Start()
    {
        timerRoutine = StartCoroutine(Timer());
        spawnRoutine = StartCoroutine(Spawn());
    }

    //타이머 관련
    IEnumerator Timer()
    {
        while (isPaused == false)
        {
            yield return oneSec;
            seconds++;
            uiManager.UpdateTimer(seconds);

            if (seconds == 30) StartCoroutine(StageClear());
        }
    }

    public void Pause_Exit()
    {
        isPaused = uiManager.Pause(isPaused);
        if (isPaused)
        {
            if (timerRoutine != null) StopCoroutine(timerRoutine);
            if (spawnRoutine != null) StopCoroutine(spawnRoutine);
            Time.timeScale = 0;
        }
        else
        {
            timerRoutine = StartCoroutine(Timer());
            spawnRoutine = StartCoroutine(Spawn());
            Time.timeScale = 1;
        }
    }

    public void Pause()
    {
        if(isPaused) return;

        isPaused = true;
        if (timerRoutine != null) StopCoroutine(timerRoutine);
        if (spawnRoutine != null) StopCoroutine(spawnRoutine);
        Time.timeScale = 0;
    }

    public void PauseOff()
    {
        if (!isPaused) return;

        isPaused = false;
        timerRoutine = StartCoroutine(Timer());
        spawnRoutine = StartCoroutine(Spawn());
        Time.timeScale = 1;
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

    //경험치 관련
    public void ExpUp(int exp)
    {
        soundManager.PlaySfx((int)StageSoundManager.StageSfx.getExp);
        curExp += exp;
        if (maxExp[maxExpIdx] <= curExp)
            levelUpRoutine = StartCoroutine(LevelUpRoutine());

        uiManager.UpdateExp(curExp, maxExp[maxExpIdx]);
    }

    //LevelUp 루틴 외부 호출용
    public void LevelUp()
    {
        if (isPaused || gameOver) return;
        if (maxExp[maxExpIdx] > curExp) return;

        levelUpRoutine = StartCoroutine(LevelUpRoutine());
    }
    
    IEnumerator LevelUpRoutine()
    {
        soundManager.PlaySfx((int)StageSoundManager.StageSfx.levelUp);
        curExp -= maxExp[maxExpIdx];
        maxExpIdx++;

        yield return levelUpSeconds;
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
