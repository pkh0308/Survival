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
    PlayerStatus baseStat;
    PlayerStatus curStat;

    //무기 정보
    Dictionary<int, WeaponData[]> weaponDic;
    Dictionary<int, WeaponData> curWeapons;
    int curMaxWeapons;

    //악세사리 정보
    Dictionary<int, AccessoryData[]> accesoryDic;
    Dictionary<int, AccessoryData> curAccessories;
    int curMaxAccessories;

    //진화 무기 정보
    Dictionary<int, WeaponData> legendaryWeaponDic;
    Dictionary<int, WeaponUpgradeData> upgradeDic;

    //누적 데미지
    Dictionary<int, int> acmDmgDic;
    int totalDmg;
    public static Action<int, int> accumulateDmg;
    public static Func<int[,]> getAccumulatedDmg;
    public static Func<int> getTotalDmg;

    //축구공
    Coroutine soccerBall;
    WaitForSeconds soccerBallSec;

    //수리검
    Coroutine shuriken;
    WaitForSeconds shurikenSec;

    //수비수
    Coroutine defender;
    WaitForSeconds defenderSec;
    List<GameObject> curDefenders;

    //로켓 미사일
    Coroutine missile;
    WaitForSeconds missileSec;

    //낙뢰
    Coroutine thunder;
    WaitForSeconds thunderSec;

    //지뢰
    Coroutine explodeMine;
    WaitForSeconds explodeMineSec;

    //양자 공
    Coroutine quantumBall;
    WaitForSeconds quantumBallSec;

    //그림자 칼날
    Coroutine shadowEdge;
    WaitForSeconds shadowEdgeSec;

    //수호자
    Coroutine guardian;
    WaitForSeconds guardianSec;

    //상어부리포
    Coroutine sharkMissile;
    WaitForSeconds sharkMissileSec;

    //천벌
    Coroutine judgement;
    WaitForSeconds judgementSec;

    //지옥불
    Coroutine hellfireMine;
    WaitForSeconds hellfireMineSec;

    void Awake()
    {
        weaponDic = new Dictionary<int, WeaponData[]>();
        curWeapons = new Dictionary<int, WeaponData>();
        accesoryDic = new Dictionary<int, AccessoryData[]>();
        curAccessories = new Dictionary<int, AccessoryData>();
        legendaryWeaponDic = new Dictionary<int, WeaponData>();
        upgradeDic = new Dictionary<int, WeaponUpgradeData>();

        acmDmgDic = new Dictionary<int, int>();
        totalDmg = 0;

        curDefenders = new List<GameObject>();

        accumulateDmg = (a, b) => { AccumulateDmg(a, b); };
        getAccumulatedDmg = () => { return GetAccumulatedDmg(); };
        getTotalDmg = () => { return totalDmg; };

        ReadData();
    }

    //csv파일에서 데이터를 읽어온 뒤 weaponDictionary에 저장
    void ReadData()
    {
        //무기 데이터 읽어오기
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
                if(id % 10 == 9) //업그레이드 무기일 경우
                {
                    string[] datas = line.Split(',');
                    // 0: id, 1: name, 2: level, 3: atk, 4: scale, 5:cooltime, 6:count, 7: projectileSpeed, 8: remainTime, 9:description
                    legendaryWeaponDic.Add(id, new WeaponData(id, datas[1], int.Parse(datas[2]), int.Parse(datas[3]), float.Parse(datas[4]),
                                                      float.Parse(datas[5]), int.Parse(datas[6]), float.Parse(datas[7]), float.Parse(datas[8]), datas[9]));

                    line = weaponDataReader.ReadLine();
                    if (line == null) break;
                    continue;
                }

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

        //악세사리 데이터 읽어오기
        TextAsset accessoryDatas = Resources.Load("AccessoryDatas") as TextAsset;
        StringReader accesoryDataReader = new StringReader(accessoryDatas.text);

        while (accesoryDataReader != null)
        {
            string line = accesoryDataReader.ReadLine();
            if (line == null) break;

            line = accesoryDataReader.ReadLine();
            while (line.Length > 1)
            {
                int curId = int.Parse(line.Split(',')[0]);
                int id = curId;
                List<AccessoryData> list = new List<AccessoryData>();

                while(curId == id)
                {
                    string[] datas = line.Split(',');
                    id = int.Parse(datas[0]);
                    if (id != curId)
                    {
                        accesoryDic.Add(curId, list.ToArray());
                        break;
                    }

                    // 0: id, 1: level, 2: name, 3: val, 4: description
                    list.Add(new AccessoryData(id, int.Parse(datas[1]), datas[2], float.Parse(datas[3]), datas[4]));

                    line = accesoryDataReader.ReadLine();
                    if (line == null) break;
                }
                if (line == null) break;
            }
        }
        accesoryDataReader.Close();

        //무기 업그레이드 데이터 읽어오기
        TextAsset weaponUpgradeDatas = Resources.Load("WeaponUpgradeDatas") as TextAsset;
        StringReader upgradeDataReader = new StringReader(weaponUpgradeDatas.text);

        while (upgradeDataReader != null)
        {
            string line = upgradeDataReader.ReadLine();
            if (line == null) break;

            line = upgradeDataReader.ReadLine();
            while (line.Length > 1)
            {
                string[] datas = line.Split(',');
                int id = int.Parse(datas[0]);
                // 0: id, 1: baseId, 2: combineId
                upgradeDic.Add(id, new WeaponUpgradeData(id, int.Parse(datas[1]), int.Parse(datas[2])));

                line = upgradeDataReader.ReadLine();
                if (line == null) break;
            }
        }
        upgradeDataReader.Close();
    }

    //Prefers 에서 플레이어 데이터 가져옴
    public void SetStatus(PlayerStatus stat)
    {
        baseStat = stat;
        curStat = stat;
    }

    public void SetObjectManager(ObjectManager manager)
    {
        objectManager = manager;
    }

    //무기 획득 or 레벨업
    public void GetWeapon(int id)
    {
        //악세사리일 경우 악세사리 획득 로직 호출
        if (id > 3000) 
        {
            GetAccesssory(id);
            return;
        }

        if(id % 10 == 9) //업그레이드 무기일 경우
        {
            //기존 무기 삭제 및 코루틴 중지
            int beforeId = id / 10 * 10 + 1;
            curWeapons.Remove(beforeId);
            StopWeaponRoutine(beforeId);
            curWeapons.Add(id, legendaryWeaponDic[id]);
            weaponDic.Remove(beforeId);

            UiManager.updateWeaponAccList(id);
            acmDmgDic.Add(id, acmDmgDic[beforeId]);
            acmDmgDic.Remove(beforeId);

            return;
        }

        if (curWeapons.ContainsKey(id))
            curWeapons[id] = weaponDic[id][curWeapons[id].WeaponLevel];
        else
        {
            curWeapons.Add(id, weaponDic[id][0]);
            UiManager.updateWeaponAccList(id);
            acmDmgDic.Add(id, 0);
        }
        //무기가 최고 레벨일 경우
        if (curWeapons[id].WeaponLevel == weaponDic[id].Length)
            curMaxWeapons++;

        if (!GameManager.IsPaused) StartWeaponRoutine(id);
    }

    public void GetAccesssory(int id)
    {
        if (curAccessories.ContainsKey(id))
            curAccessories[id] = accesoryDic[id][curAccessories[id].AccessoryLevel];
        else
        {
            curAccessories.Add(id, accesoryDic[id][0]);
            UiManager.updateWeaponAccList(id);
        }
        
        curStat.SetStatus(id, baseStat, curAccessories[id].AccessoryValue);
        //악세사리가 최고 레벨일 경우
        if (curAccessories[id].AccessoryLevel == accesoryDic[id].Length)
            curMaxAccessories++;
    }

    //레벨업 시 호출
    public List<DataForLevelUp> GetRandomWeaponData()
    {
        List<DataForLevelUp> datas = new List<DataForLevelUp>();
        //무기, 악세사리 키 병합
        List<int> keys = new List<int>();
        if(curMaxWeapons < 6) 
            keys.AddRange(weaponDic.Keys);
        if (curMaxAccessories < 6) 
            keys.AddRange(accesoryDic.Keys);  

        //모든 무기와 악세사리가 최고레벨일 경우 골드/회복 옵션 노출
        if (keys.Count == 0)
        {
            //체력 회복, 골드 획득 옵션
            datas.Add(new DataForLevelUp(weaponDic[ObjectNames.meat_50][0]));
            datas.Add(new DataForLevelUp(weaponDic[ObjectNames.gold_70][0]));
            return datas;
        }
        //레벨업 가능한 무기가 있을 경우 목록에서 골드/회복 옵션 삭제
        keys.Remove(ObjectNames.meat_50);
        keys.Remove(ObjectNames.gold_70);

        while (datas.Count < 3)
        {
            int id = keys[Random.Range(0, keys.Count)];
            if (id < 3000) //무기
            {
                if (curWeapons.TryGetValue(id, out WeaponData w_data) == false)
                    datas.Add(new DataForLevelUp(weaponDic[id][0]));
                else if (w_data.WeaponLevel < weaponDic[id].Length)
                    datas.Add(new DataForLevelUp(weaponDic[id][w_data.WeaponLevel]));
                else //가지고 있는 무기이며, 최대 레벨일 경우
                {
                    int tempId = (id / 10 * 10) + 9; //업그레이드 무기 id
                    if (!legendaryWeaponDic.ContainsKey(tempId))  //업그레이드 무기가 없을 경우
                        continue;

                    if(curAccessories.ContainsKey(upgradeDic[tempId].combineId))
                        datas.Add(new DataForLevelUp(legendaryWeaponDic[tempId]));
                }
            }
            else // 악세사리
            {
                if (curAccessories.TryGetValue(id, out AccessoryData a_data) == false)
                    datas.Add(new DataForLevelUp(accesoryDic[id][0]));
                else if (a_data.AccessoryLevel < accesoryDic[id].Length)
                    datas.Add(new DataForLevelUp(accesoryDic[id][a_data.AccessoryLevel]));
            }

            keys.Remove(id);
            //레벨업 가능한 무기나 악세사리가 없을 경우 루프 탈출
            if (keys.Count < 1) break;
        }
        return datas;
    }

    //보물상자 획득 시 데이터 전달
    public DataForLevelUp[] GetLotteryWeaponData(out int targetId)
    {
        List<DataForLevelUp> datas = new List<DataForLevelUp>();
        targetId = -1;

        //업그레이드 가능한 무기가 있는지 확인
        //보물상자의 경우 업그레이드 가능한 무기 확정 획득
        if (HasUpgradable(out int upgradeId))
        {
            datas.Add(new DataForLevelUp(legendaryWeaponDic[upgradeId]));
            targetId = upgradeId;
        }

        //무기, 악세사리 키 병합
        List<int> keys = new List<int>();
        if (curMaxWeapons < 6)
            keys.AddRange(curWeapons.Keys);
        if (curMaxAccessories < 6)
            keys.AddRange(curAccessories.Keys);

        //모든 무기와 악세사리가 최고레벨일 경우 골드/회복 옵션 노출
        if (keys.Count == 0)
        {
            //체력 회복, 골드 획득 옵션
            keys.Add(ObjectNames.meat_50);
            keys.Add(ObjectNames.gold_70);
        }
        else //레벨업 가능한 무기가 있을 경우 목록에서 골드/회복 옵션 삭제
        {
            keys.Remove(ObjectNames.meat_50);
            keys.Remove(ObjectNames.gold_70);
        }

        while(datas.Count < ObjectNames.maxLotterySlot)
        {
            int id = keys[Random.Range(0, keys.Count)];
            if (id < 3000) //무기
            {
                if(id % 10 == 9) //업그레이드 무기일 경우
                    datas.Add(new DataForLevelUp(legendaryWeaponDic[id]));
                else if (curWeapons[id].WeaponLevel < weaponDic[id].Length)
                    datas.Add(new DataForLevelUp(weaponDic[id][curWeapons[id].WeaponLevel]));
            }
            else if(id < 4000)// 악세사리
            {
                if (curAccessories[id].AccessoryLevel < accesoryDic[id].Length)
                    datas.Add(new DataForLevelUp(accesoryDic[id][curAccessories[id].AccessoryLevel]));
            }
            else //골드 or 회복 옵션
            {
                datas.Add(new DataForLevelUp(weaponDic[id][curWeapons[id].WeaponLevel]));
            }
        }

        return datas.ToArray();
    }

    //업그레이드 가능한 무기가 있는지 확인
    //없을 경우 false, id는 -1을 반환
    bool HasUpgradable(out int id)
    {
        List<int> keys = new List<int>(curWeapons.Keys);
        for(int i = 0; i < keys.Count; i++)
        {
            int idx = keys[i];
            if (curWeapons[idx].WeaponLevel < 5)
                continue;

            int tempId = (curWeapons[idx].WeaponId / 10 * 10) + 9; //업그레이드 무기 id
            if (!legendaryWeaponDic.ContainsKey(tempId))  //업그레이드 무기가 없을 경우
                continue;

            if (curAccessories.ContainsKey(upgradeDic[tempId].combineId)) //조합 악세사리도 보유중일 경우
            {
                id = tempId;
                return true;
            }
        }
        id = -1;
        return false;
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
            //일반 무기
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
            case ObjectNames.missile:
                if (missile != null) StopCoroutine(missile);
                missile = StartCoroutine(Missile(curWeapons[id].WeaponCooltime));
                break;
            case ObjectNames.thunder:
                if (thunder != null) StopCoroutine(thunder);
                thunder = StartCoroutine(Thunder(curWeapons[id].WeaponCooltime));
                break;
            case ObjectNames.explodeMine:
                if (explodeMine != null) StopCoroutine(explodeMine);
                explodeMine = StartCoroutine(ExplodeMine(curWeapons[id].WeaponCooltime));
                break;
            //업그레이드 무기
            case ObjectNames.quantumBall:
                if (quantumBall != null) StopCoroutine(quantumBall);
                quantumBall = StartCoroutine(QuantumBall(curWeapons[id].WeaponCooltime));
                break;
            case ObjectNames.shadowEdge:
                if (shadowEdge != null) StopCoroutine(shadowEdge);
                shadowEdge = StartCoroutine(ShadowEdge());
                break;
            case ObjectNames.guardian:
                if (guardian != null) break;
                guardian = StartCoroutine(Guardian());
                break;
            case ObjectNames.sharkMissile:
                if (sharkMissile != null) StopCoroutine(sharkMissile);
                sharkMissile = StartCoroutine(SharkMissile(curWeapons[id].WeaponCooltime));
                break;
            case ObjectNames.judgement:
                if (judgement != null) StopCoroutine(judgement);
                judgement = StartCoroutine(Judgement(curWeapons[id].WeaponCooltime));
                break;
            case ObjectNames.hellfireMine:
                if (hellfireMine != null) StopCoroutine(hellfireMine);
                hellfireMine = StartCoroutine(HellfireMine(curWeapons[id].WeaponCooltime));
                break;
        }
    }

    void StopWeaponRoutine(int id)
    {
        switch (id)
        {
            //일반 무기
            case ObjectNames.soccerBall:
                StopCoroutine(soccerBall);
                break;
            case ObjectNames.shuriken:
                StopCoroutine(shuriken);
                break;
            case ObjectNames.defender:
                StopCoroutine(defender);
                break;
            case ObjectNames.missile:
                StopCoroutine(missile);
                break;
            case ObjectNames.thunder:
                StopCoroutine(thunder);
                break;
            case ObjectNames.explodeMine:
                StopCoroutine(explodeMine);
                break;
            //업그레이드 무기
            case ObjectNames.quantumBall:
                StopCoroutine(quantumBall);
                break;
            case ObjectNames.shadowEdge:
                StopCoroutine(shadowEdge);
                break;
            case ObjectNames.guardian:
                StopCoroutine(guardian);
                break;
            case ObjectNames.sharkMissile:
                StopCoroutine(sharkMissile);
                break;
            case ObjectNames.judgement:
                StopCoroutine(judgement);
                break;
            case ObjectNames.hellfireMine:
                StopCoroutine(hellfireMine);
                break;
        }
    }

    // 축구공
    IEnumerator SoccerBall(float cooldown)
    {
        soccerBallSec = new WaitForSeconds(cooldown * curStat.CoolTimeVal);
        int maxProj = curWeapons[ObjectNames.soccerBall].WeaponProjectileCount + curStat.ProjCountVal;

        while (!GameManager.IsPaused)
        {
            for (int i = 0; i < maxProj; i++)
            {
                GameObject ball = objectManager.MakeObj(ObjectNames.soccerBall);
                ball.transform.position = transform.position;
                ball.GetComponent<SoccerBall>().Initialize(curWeapons[ObjectNames.soccerBall],
                                                                  curStat.AtkPowerVal, curStat.AtkScaleVal, curStat.ProjSpeedVal, curStat.AtkRemainTimeVal);
            }
            yield return soccerBallSec;
        }
    }

    // 수리검
    IEnumerator Shuriken(float cooldown)
    {
        WaitForSeconds projInterval = new WaitForSeconds(0.1f);
        int maxProj = curWeapons[ObjectNames.shuriken].WeaponProjectileCount + curStat.ProjCountVal;
        shurikenSec = new WaitForSeconds(cooldown * curStat.CoolTimeVal - (0.1f * maxProj));

        while (!GameManager.IsPaused)
        {
            for (int i = 0; i < maxProj; i++)
            {
                GameObject shk = objectManager.MakeObj(ObjectNames.shuriken);
                shk.transform.position = transform.position;
                shk.GetComponent<Shuriken>().Initialize(curWeapons[ObjectNames.shuriken],
                                                        curStat.AtkPowerVal, curStat.AtkScaleVal, curStat.ProjSpeedVal, curStat.AtkRemainTimeVal);

                yield return projInterval;
            }

            yield return shurikenSec;
        }
    }

    // 수호자
    IEnumerator Defender(float cooldown)
    {
        defenderSec = new WaitForSeconds(cooldown * curStat.CoolTimeVal);
        int maxProj = curWeapons[ObjectNames.defender].WeaponProjectileCount + curStat.ProjCountVal;
        //이미 생성된 수호자가 있을 경우 초기화
        if(curDefenders.Count > 0)
        {
            foreach (GameObject obj in curDefenders)
                obj.SetActive(false);
            curDefenders.Clear();
        }

        while (!GameManager.IsPaused)
        {
            float rotateOffset = 360 / curWeapons[ObjectNames.defender].WeaponProjectileCount;

            curDefenders.Clear();
            for (int i = 0; i < maxProj; i++)
            {
                curDefenders.Add(objectManager.MakeObj(ObjectNames.defender));
                curDefenders[i].transform.position = transform.position + (Vector3.up * 2);
                curDefenders[i].transform.RotateAround(transform.position, Vector3.forward, rotateOffset * i);
                curDefenders[i].GetComponent<Defender>().Initialize(curWeapons[ObjectNames.defender],
                                                        curStat.AtkPowerVal, curStat.AtkScaleVal, curStat.ProjSpeedVal, curStat.AtkRemainTimeVal);
            }
            yield return defenderSec;
        }
    }

    //로켓 미사일
    IEnumerator Missile(float cooldown)
    {
        WaitForSeconds projInterval = new WaitForSeconds(0.1f);
        int maxProj = curWeapons[ObjectNames.missile].WeaponProjectileCount + curStat.ProjCountVal;
        missileSec = new WaitForSeconds(cooldown * curStat.CoolTimeVal - (0.1f * maxProj));

        while (!GameManager.IsPaused)
        {
            for (int i = 0; i < maxProj; i++)
            {
                GameObject mis = objectManager.MakeObj(ObjectNames.missile);
                mis.transform.position = transform.position;
                mis.GetComponent<Missile>().Initialize(curWeapons[ObjectNames.missile],
                                                        curStat.AtkPowerVal, curStat.AtkScaleVal, curStat.ProjSpeedVal, curStat.AtkRemainTimeVal);

                yield return projInterval;
            }

            yield return missileSec;
        }
    }

    //낙뢰
    IEnumerator Thunder(float cooldown)
    {
        WaitForSeconds projInterval = new WaitForSeconds(0.1f);
        int maxProj = curWeapons[ObjectNames.thunder].WeaponProjectileCount + curStat.ProjCountVal;
        thunderSec = new WaitForSeconds(cooldown * curStat.CoolTimeVal - (0.1f * maxProj));
        
        while (!GameManager.IsPaused)
        {
            for (int i = 0; i < maxProj; i++)
            {
                GameObject thd = objectManager.MakeObj(ObjectNames.thunder);
                thd.GetComponent<Thunder>().Initialize(curWeapons[ObjectNames.thunder],
                                                        curStat.AtkPowerVal, curStat.AtkScaleVal, curStat.ProjSpeedVal, curStat.AtkRemainTimeVal);

                yield return projInterval;
            }

            yield return thunderSec;
        }
    }

    //지뢰
    IEnumerator ExplodeMine(float cooldown)
    {
        explodeMineSec = new WaitForSeconds(cooldown * curStat.CoolTimeVal);
        int maxProj = curWeapons[ObjectNames.explodeMine].WeaponProjectileCount + curStat.ProjCountVal;

        while (!GameManager.IsPaused)
        {
            for (int i = 0; i < maxProj; i++)
            {
                GameObject mine = objectManager.MakeObj(ObjectNames.explodeMine);
                mine.GetComponent<ExplodeMine>().Initialize(curWeapons[ObjectNames.explodeMine],
                                                                  curStat.AtkPowerVal, curStat.AtkScaleVal, curStat.ProjSpeedVal, curStat.AtkRemainTimeVal);
            }
            yield return explodeMineSec;
        }
    }

    /////// 업그레이드 무기 ///////
    //양자 공
    IEnumerator QuantumBall(float cooldown)
    {
        quantumBallSec = new WaitForSeconds(cooldown * curStat.CoolTimeVal);
        int maxProj = curWeapons[ObjectNames.quantumBall].WeaponProjectileCount + curStat.ProjCountVal;

        while (!GameManager.IsPaused)
        {
            for (int i = 0; i < maxProj; i++)
            {
                GameObject ball = objectManager.MakeObj(ObjectNames.quantumBall);
                ball.transform.position = transform.position;
                ball.GetComponent<SoccerBall>().Initialize(curWeapons[ObjectNames.quantumBall],
                                                                  curStat.AtkPowerVal, curStat.AtkScaleVal, curStat.ProjSpeedVal, curStat.AtkRemainTimeVal);
            }
            yield return quantumBallSec;
        }
    }

    //그림자 칼날
    IEnumerator ShadowEdge()
    {
        WaitForSeconds projInterval = new WaitForSeconds(0.1f);

        while (!GameManager.IsPaused)
        {
            GameObject shd = objectManager.MakeObj(ObjectNames.shadowEdge);
            shd.transform.position = transform.position;
            shd.GetComponent<Shuriken>().Initialize(curWeapons[ObjectNames.shadowEdge],
                                                    curStat.AtkPowerVal, curStat.AtkScaleVal, curStat.ProjSpeedVal, curStat.AtkRemainTimeVal);

            yield return projInterval;
        }
    }

    //수호자
    IEnumerator Guardian()
    {
        int maxProj = curWeapons[ObjectNames.guardian].WeaponProjectileCount + curStat.ProjCountVal;
        float rotateOffset = 360 / maxProj;

        for (int i = 0; i < maxProj; i++)
        {
            GameObject guard = objectManager.MakeObj(ObjectNames.guardian);
            guard.transform.position = transform.position + (Vector3.up * 2);
            guard.transform.RotateAround(transform.position, Vector3.forward, rotateOffset * i);
            guard.GetComponent<Guardian>().Initialize(curWeapons[ObjectNames.guardian],
                                                    curStat.AtkPowerVal, curStat.AtkScaleVal, curStat.ProjSpeedVal, curStat.AtkRemainTimeVal);
        }

        guardianSec = new WaitForSeconds(curWeapons[ObjectNames.guardian].WeaponCooltime);
        while (gameObject.activeSelf)
        {
            if(!GameManager.IsPaused)
                StageSoundManager.playWeaponSfx((int)StageSoundManager.WeaponSfx.defender);
            yield return guardianSec;
        }
    }

    //상어부리포
    IEnumerator SharkMissile(float cooldown)
    {
        WaitForSeconds projInterval = new WaitForSeconds(0.1f);
        int maxProj = curWeapons[ObjectNames.sharkMissile].WeaponProjectileCount + curStat.ProjCountVal;
        sharkMissileSec = new WaitForSeconds(cooldown * curStat.CoolTimeVal - (0.1f * maxProj));

        while (!GameManager.IsPaused)
        {
            for (int i = 0; i < maxProj; i++)
            {
                GameObject mis = objectManager.MakeObj(ObjectNames.sharkMissile);
                mis.transform.position = transform.position;
                mis.GetComponent<Missile>().Initialize(curWeapons[ObjectNames.sharkMissile],
                                                        curStat.AtkPowerVal, curStat.AtkScaleVal, curStat.ProjSpeedVal, curStat.AtkRemainTimeVal);

                yield return projInterval;
            }
            yield return sharkMissileSec;
        }
    }

    //천벌
    IEnumerator Judgement(float cooldown)
    {
        WaitForSeconds projInterval = new WaitForSeconds(0.1f);
        int maxProj = curWeapons[ObjectNames.judgement].WeaponProjectileCount + curStat.ProjCountVal;
        judgementSec = new WaitForSeconds(cooldown * curStat.CoolTimeVal - (0.1f * maxProj));

        while (!GameManager.IsPaused)
        {
            for (int i = 0; i < maxProj; i++)
            {
                GameObject jud = objectManager.MakeObj(ObjectNames.judgement);
                jud.GetComponent<Thunder>().Initialize(curWeapons[ObjectNames.judgement],
                                                        curStat.AtkPowerVal, curStat.AtkScaleVal, curStat.ProjSpeedVal, curStat.AtkRemainTimeVal);

                yield return projInterval;
            }
            yield return judgementSec;
        }
    }

    //불지옥
    IEnumerator HellfireMine(float cooldown)
    {
        hellfireMineSec = new WaitForSeconds(cooldown * curStat.CoolTimeVal);
        int maxProj = curWeapons[ObjectNames.hellfireMine].WeaponProjectileCount + curStat.ProjCountVal;

        while (!GameManager.IsPaused)
        {
            for (int i = 0; i < maxProj; i++)
            {
                GameObject mine = objectManager.MakeObj(ObjectNames.hellfireMine);
                mine.GetComponent<ExplodeMine>().Initialize(curWeapons[ObjectNames.hellfireMine],
                                                                  curStat.AtkPowerVal, curStat.AtkScaleVal, curStat.ProjSpeedVal, curStat.AtkRemainTimeVal);
            }
            yield return hellfireMineSec;
        }
    }

    public void AllStop()
    {
        StopAllCoroutines();
    }
}