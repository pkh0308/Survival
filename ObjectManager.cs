using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    //무기 투사체
    [SerializeField] GameObject soccerBallPrefab;
    GameObject[] soccerBall;

    //적
    [SerializeField] GameObject enemyMeleePrefab;
    GameObject[] enemyMelee;

    //경험치 젬
    [SerializeField] GameObject expLowPrefab;
    [SerializeField] GameObject expMiddlePrefab;
    [SerializeField] GameObject expHighPrefab;
    GameObject[] expGemLow;
    GameObject[] expGemMiddle;
    GameObject[] expGemHigh;

    GameObject[] targetPool;

    public static Func<int, GameObject> dropExp;

    void Awake()
    {
        dropExp = (a) => { return DropExp(a); };

        soccerBall = new GameObject[30];
        enemyMelee = new GameObject[100];

        expGemLow = new GameObject[1000];
        expGemMiddle = new GameObject[1000];
        expGemHigh = new GameObject[1000];

        Generate();
    }

    // 스테이지 로딩후 필요 개체들 미리 생성
    // 오퍼레이터의 경우 operatorStatus 파일을 읽어와서 스테이터스 설정
    void Generate()
    {
        //파일에서 데이터 읽어오기

        //무기 투사체
        for (int idx = 0; idx < soccerBall.Length; idx++)
        {
            soccerBall[idx] = Instantiate(soccerBallPrefab);
            soccerBall[idx].SetActive(false);
        }

        //경험치 젬
        for (int idx = 0; idx < expGemLow.Length; idx++)
        {
            expGemLow[idx] = Instantiate(expLowPrefab);
            expGemLow[idx].SetActive(false);
        }
        for (int idx = 0; idx < expGemMiddle.Length; idx++)
        {
            expGemMiddle[idx] = Instantiate(expMiddlePrefab);
            expGemMiddle[idx].SetActive(false);
        }
        for (int idx = 0; idx < expGemHigh.Length; idx++)
        {
            expGemHigh[idx] = Instantiate(expHighPrefab);
            expGemHigh[idx].SetActive(false);
        }
    }

    public GameObject MakeObj(int id)
    {
        switch (id)
        {
            case 1001:
                targetPool = soccerBall;
                break;
            case 100001:
                targetPool = enemyMelee;
                break;
        }

        for (int idx = 0; idx < targetPool.Length; idx++)
        {
            if (!targetPool[idx].activeSelf)
            {
                targetPool[idx].SetActive(true);
                return targetPool[idx];
            }
        }
        return null;
    }

    public GameObject DropExp(int grade)
    {
        switch (grade)
        {
            case 0:
                targetPool = expGemLow;
                break;
            case 1:
                targetPool = expGemMiddle;
                break;
            case 2:
                targetPool = expGemHigh;
                break;
        }

        for (int idx = 0; idx < targetPool.Length; idx++)
        {
            if (!targetPool[idx].activeSelf)
            {
                targetPool[idx].SetActive(true);
                return targetPool[idx];
            }
        }
        return null;
    }

    public GameObject[] GetPool(string type)
    {
        switch (type)
        {
            case "enemyMelee":
                targetPool = enemyMelee;
                break;
        }
        return targetPool;
    }

    public GameObject GetOne(string type)
    {
        switch (type)
        {
            case "enemyMelee":
                targetPool = enemyMelee;
                break;
        }
        return targetPool[0];
    }

    public int GetActiveCount(string type)
    {
        int count = 0;

        switch (type)
        {
            case "enemyMelee":
                targetPool = enemyMelee;
                break;
        }

        for (int idx = 0; idx < targetPool.Length; idx++)
        {
            if (targetPool[idx].activeSelf)
            {
                count++;
            }
        }
        return count;
    }

}
