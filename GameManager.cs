using System;
using System.Collections;
using System.Collections.Generic;
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

    void Awake()
    {
        oneSec = new WaitForSeconds(1.0f);
        seconds = 0;

        spawnSec = new WaitForSeconds(0.3f);

        isPaused = false;

        maxExp = Enumerable.Repeat<int>(100, 100).ToArray();
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
        }
    }

    public void Pause()
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

    public void PauseOff()
    {
        isPaused = false;
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
        {
            while(maxExp[maxExpIdx] <= curExp)
            {
                curExp -= maxExp[maxExpIdx];
                maxExpIdx++;
                LevelUp();
            }
            uiManager.UpdateLevel(maxExpIdx + 1);
        }
        uiManager.UpdateExp(curExp, maxExp[maxExpIdx]);
    }

    void LevelUp()
    {
        isPaused = true;
        uiManager.WeaponSelect();
    }

    //게임 오버
    public void GameOver()
    {

    }
}
