using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] UiManager uiManager;

    static bool isPaused;
    public static bool IsPaused { get { return isPaused; } }

    //타이머 관련
    WaitForSeconds oneSec;
    int seconds;
    Coroutine timerRoutine;

    //적 스폰 관련
    WaitForSeconds spawnSec;
    Coroutine spawnRoutine;

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

        spawnSec = new WaitForSeconds(0.3f);

        levelUpSeconds = new WaitForSeconds(levelUpInterval);

        isPaused = false;

        maxExp = Enumerable.Repeat<int>(100, 100).ToArray();

        killCountPlus = () => { UpdateKillCount(); };
        moneyCountPlus = (a) => { UpdateMoneyCount(a); };
    }

    void Start()
    {
        timerRoutine = StartCoroutine(Timer());
        spawnRoutine = StartCoroutine(Spawn());
    }

    //타이머 관련
    IEnumerator Timer()
    {
        while(isPaused == false)
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
            StopCoroutine(timerRoutine);
            StopCoroutine(spawnRoutine);
        }
        else
        {
            timerRoutine = StartCoroutine(Timer());
            spawnRoutine = StartCoroutine(Spawn());
        }
    }

    public void Pause()
    {
        isPaused = true;
        StopCoroutine(timerRoutine);
        StopCoroutine(spawnRoutine);
    }

    public void PauseOff()
    {
        isPaused = false;
        timerRoutine = StartCoroutine(Timer());
        spawnRoutine = StartCoroutine(Spawn());
    }

    //적 스폰 관련
    IEnumerator Spawn()
    {
        while(!isPaused)
        {
            yield return spawnSec;
        }
    }

    //경험치 관련
    public void ExpUp(int exp)
    {
        curExp += exp;
        if(maxExp[maxExpIdx] <= curExp)
            levelUpRoutine = StartCoroutine(LevelUpRoutine());

        uiManager.UpdateExp(curExp, maxExp[maxExpIdx]);
    }

    //LevelUp 루틴 외부 호출용
    public void LevelUp()
    {
        if (maxExp[maxExpIdx] > curExp) return;

        levelUpRoutine = StartCoroutine(LevelUpRoutine());
    }
    
    IEnumerator LevelUpRoutine()
    {
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
        //모든 적 사망처리

        yield return new WaitForSeconds(3.0f);
        isPaused = true;
        uiManager.StageClear(killCount, moneyCount);
    }

    //게임 오버
    public void GameOver()
    {
        isPaused = true;
        uiManager.GameOver(killCount, moneyCount);
    }
}
