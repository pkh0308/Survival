using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Random = UnityEngine.Random; //System.Random 과 혼선 방지용

public class Weapons : MonoBehaviour
{
    ObjectManager objectManager;

    //캐릭터 스탯
    PlayerStatus stat;

    //무기 정보
    Dictionary<int, WeaponData[]> weaponDic;

    //보유중인 무기 슬롯
    Dictionary<int, WeaponData> curWeapons;
    int curMaxWeapons;

    //누적 데미지
    Dictionary<int, int> acmDmgDic;
    int totalDmg;
    public static Action<int, int> accumulateDmg;
    public static Func<int[,]> getAccumulatedDmg;
    public static Func<int> getTotalDmg;

    //축구공
    [Header("축구공")]
    Coroutine soccerBall;
    WaitForSeconds soccerBallSec;

    //수리검
    [Header("수리검")]
    Coroutine shuriken;
    WaitForSeconds shurikenSec;

    //수호자
    [Header("수호자")]
    Coroutine defender;
    WaitForSeconds defenderSec;

    void Awake()
    {
        weaponDic = new Dictionary<int, WeaponData[]>();
        curWeapons = new Dictionary<int, WeaponData>();
        acmDmgDic = new Dictionary<int, int>();
        totalDmg = 0;

        accumulateDmg = (a, b) => { AccumulateDmg(a, b); };
        getAccumulatedDmg = () => { return GetAccumulatedDmg(); };
        getTotalDmg = () => { return totalDmg; };

        ReadData();
    }

    //csv파일에서 데이터를 읽어온 뒤 weaponDictionary에 저장
    void ReadData()
    {
        //csv 파일에서 데이터 읽어오기
        TextAsset weaponDatas = Resources.Load("WeaponDatas") as TextAsset;
        StringReader weaponDataReader = new StringReader(weaponDatas.text);

        while (weaponDataReader != null)
        {
            string line = weaponDataReader.ReadLine();
            if (line == null) break;

            line = weaponDataReader.ReadLine();
            while (line.Length > 1)
            {
                int id = int.Parse(line.Split(',')[0]);
                if(id == ObjectNames.meat_50 || id == ObjectNames.gold_70)
                    weaponDic.Add(id, new WeaponData[1]);
                else
                    weaponDic.Add(id, new WeaponData[5]);
                
                for(int i = 0; i < weaponDic[id].Length; i++)
                {
                    string[] datas = line.Split(',');
                    // 0: id, 1: name, 2: level, 3: atk, 4: scale, 5:cooltime, 6:count, 7: projectileSpeed, 8: remainTime, 9:description
                    weaponDic[id][i] = new WeaponData(id, datas[1], int.Parse(datas[2]), int.Parse(datas[3]), float.Parse(datas[4]), 
                                                      float.Parse(datas[5]), int.Parse(datas[6]), float.Parse(datas[7]), float.Parse(datas[8]), datas[9]);
                    
                    line = weaponDataReader.ReadLine();
                    if (line == null) break;
                }
                if (line == null) break;
            }
        }
        weaponDataReader.Close();
    }

    //Prefers 에서 플레이어 데이터 가져옴
    public void SetStatus(PlayerStatus stat)
    {
        this.stat = stat;
    }

    public void SetObjectManager(ObjectManager manager)
    {
        objectManager = manager;
    }

    //무기 획득 or 레벨업
    public void GetWeapon(int id)
    {
        if (curWeapons.ContainsKey(id))
            curWeapons[id] = weaponDic[id][curWeapons[id].WeaponLevel];
        else
        {
            curWeapons.Add(id, weaponDic[id][0]);
            acmDmgDic.Add(id, 0);
        }

        if (curWeapons[id].WeaponLevel == 5)
            curMaxWeapons++;

        StartWeaponRoutine(id);
    }

    //레벨업, 상자 획득 시 호출
    public List<WeaponData> GetRandomWeaponData()
    {
        List<WeaponData> datas = new List<WeaponData>();
        List<int> keys = new List<int>(weaponDic.Keys);

        if (curMaxWeapons >= 6)
        {
            //체력 회복, 골드 획득 옵션
            datas.Add(weaponDic[ObjectNames.meat_50][0]);
            datas.Add(weaponDic[ObjectNames.gold_70][0]);
            return datas;
        }

        keys.Remove(ObjectNames.meat_50);
        keys.Remove(ObjectNames.gold_70);
        while (datas.Count < 3)
        {
            int id = keys[Random.Range(0, keys.Count)];
            if (curWeapons.TryGetValue(id, out WeaponData data) == false)
                datas.Add(weaponDic[id][0]);
            else if (data.WeaponLevel < 5)
                datas.Add(weaponDic[id][data.WeaponLevel]);

            keys.Remove(id);
            //레벨업 가능한 무기가 없을 경우 루프 탈출
            if (keys.Count < 1) break;
        }

        return datas;
    }

    //일시정지 종료 후 무기 코루틴 재시작용
    public void RestartWeapons()
    {
        List<int> keys = new List<int>(curWeapons.Keys);
        for(int i = 0; i < keys.Count; i++)
            StartWeaponRoutine(keys[i]);
    }

    //데미지 누적
    public void AccumulateDmg(int id, int val)
    {
        //Dictionary 등록은 무기 획득 시 실행, 등록되지 않은 id라면 디버그 로그 실행
        if (acmDmgDic.ContainsKey(id))
            acmDmgDic[id] += val;
        else
            Debug.Log("해당 id의 무기가 등록되지 않았습니다 : " + id);

        totalDmg += val;
    }

    public int[,] GetAccumulatedDmg()
    {
        List<int> keys = acmDmgDic.Keys.ToList();
        int[,] arr = new int[keys.Count, 2];

        for(int i = 0; i < keys.Count; i++)
        {
            arr[i, 0] = keys[i];
            arr[i, 1] = acmDmgDic[keys[i]];
        }
        return arr;
    }

    //신규 무기 획득 시 호출
    //혼선 방지를 위해 해당 무기의 코루틴이 null이 아니면 정지 후 재시작
    void StartWeaponRoutine(int id)
    {
        switch(id)
        {
            case ObjectNames.soccerBall:
                if (soccerBall != null) StopCoroutine(soccerBall);
                soccerBall = StartCoroutine(SoccerBall(curWeapons[id].WeaponCooltime));
                break;
            case ObjectNames.shuriken:
                if (shuriken != null) StopCoroutine(shuriken);
                shuriken = StartCoroutine(Shuriken(curWeapons[id].WeaponCooltime));
                break;
            case ObjectNames.defender:
                if (defender != null) StopCoroutine(defender);
                defender = StartCoroutine(Defender(curWeapons[id].WeaponCooltime));
                break;
        }
    }

    // 축구공
    IEnumerator SoccerBall(float cooldown)
    {
        soccerBallSec = new WaitForSeconds(cooldown * stat.CoolTimeVal);
        List<GameObject> ballList = new List<GameObject>();
        
        while(!GameManager.IsPaused)
        {
            ballList.Clear();
            int maxProj = curWeapons[ObjectNames.soccerBall].WeaponProjectileCount + stat.ProjCountVal;
            for (int i = 0; i < maxProj; i++)
            {
                ballList.Add(objectManager.MakeObj(ObjectNames.soccerBall));
                ballList[i].transform.position = transform.position;
                ballList[i].GetComponent<SoccerBall>().Initialize(curWeapons[ObjectNames.soccerBall], 
                                                                  stat.AtkPowerVal, stat.AtkScaleVal, stat.ProjSpeedVal, stat.AtkRemainTimeVal);
            }
            
            yield return soccerBallSec;
            foreach(GameObject obj in ballList)
                obj.SetActive(false);
        }
    }

    // 수리검
    IEnumerator Shuriken(float cooldown)
    {
        WaitForSeconds projInterval = new WaitForSeconds(0.1f);

        while (!GameManager.IsPaused)
        {
            int maxProj = curWeapons[ObjectNames.shuriken].WeaponProjectileCount + stat.ProjCountVal;
            shurikenSec = new WaitForSeconds(cooldown * stat.CoolTimeVal - (0.1f * maxProj));
            for (int i = 0; i < maxProj; i++)
            {
                GameObject shk = objectManager.MakeObj(ObjectNames.shuriken);
                shk.transform.position = transform.position;
                shk.GetComponent<Shuriken>().Initialize(curWeapons[ObjectNames.shuriken],
                                                        stat.AtkPowerVal, stat.AtkScaleVal, stat.ProjSpeedVal, stat.AtkRemainTimeVal);

                yield return projInterval;
            }

            yield return shurikenSec;
        }
    }

    // 수호자
    IEnumerator Defender(float cooldown)
    {
        defenderSec = new WaitForSeconds(cooldown * stat.CoolTimeVal);

        while (!GameManager.IsPaused)
        {
            float rotateOffset = 360 / curWeapons[ObjectNames.defender].WeaponProjectileCount;

            int maxProj = curWeapons[ObjectNames.defender].WeaponProjectileCount + stat.ProjCountVal;
            for (int i = 0; i < maxProj; i++)
            {
                GameObject def = objectManager.MakeObj(ObjectNames.defender);
                def.transform.position = transform.position + (Vector3.up * 2);
                def.transform.RotateAround(transform.position, Vector3.forward, rotateOffset * i);
                def.GetComponent<Defender>().Initialize(curWeapons[ObjectNames.defender], 
                                                        stat.AtkPowerVal, stat.AtkScaleVal, stat.ProjSpeedVal, stat.AtkRemainTimeVal);
            }

            yield return defenderSec;
            yield return defenderSec;
        }
    }

    public void AllStop()
    {
        StopAllCoroutines();
    }
}
