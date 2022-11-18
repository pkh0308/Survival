using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapons : MonoBehaviour
{
    [SerializeField] ObjectManager objectManager;

    //무기 정보
    Dictionary<int, WeaponData[]> weaponDic;

    //보유중인 무기 슬롯
    Dictionary<int, WeaponData> curWeapons;
    int curMaxWeapons;

    //축구공
    Coroutine soccerBall;
    WaitForSeconds soccerBallSec;

    //수리검
    Coroutine shuriken;
    WaitForSeconds shurikenSec;

    //수호자
    Coroutine defender;
    WaitForSeconds defenderSec;

    void Awake()
    {
        weaponDic = new Dictionary<int, WeaponData[]>();
        curWeapons = new Dictionary<int, WeaponData>();

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
                if(id == ObjectNames.meet_50 || id == ObjectNames.gold_70)
                    weaponDic.Add(id, new WeaponData[1]);
                else
                    weaponDic.Add(id, new WeaponData[5]);
                
                for(int i = 0; i < weaponDic[id].Length; i++)
                {
                    string[] datas = line.Split(',');
                    // 0: id, 1: name, 2: level, 3: atk, 4: scale, 5:cooltime, 6:count, 7: projectileSpeed, 8:description
                    weaponDic[id][i] = new WeaponData(id, datas[1], int.Parse(datas[2]), int.Parse(datas[3]), float.Parse(datas[4]), 
                                                      float.Parse(datas[5]), int.Parse(datas[6]), float.Parse(datas[7]), datas[8]);
                    
                    line = weaponDataReader.ReadLine();
                    if (line == null) break;
                }
                if (line == null) break;
            }
        }
        weaponDataReader.Close();
    }

    //무기 획득 or 레벨업
    public void GetWeapon(int id)
    {
        if (curWeapons.TryGetValue(id, out WeaponData w))
            curWeapons[id] = weaponDic[id][curWeapons[id].WeaponLevel];
        else
            curWeapons.Add(id, weaponDic[id][0]);

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
            datas.Add(weaponDic[ObjectNames.meet_50][0]);
            datas.Add(weaponDic[ObjectNames.gold_70][0]);
            return datas;
        }

        keys.Remove(ObjectNames.meet_50);
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

    IEnumerator SoccerBall(float cooltime)
    {
        soccerBallSec = new WaitForSeconds(cooltime);
        List<GameObject> ballList = new List<GameObject>();
        
        while(!GameManager.IsPaused)
        {
            ballList.Clear();
            for (int i = 0; i < curWeapons[ObjectNames.soccerBall].WeaponProjectileCount; i++)
            {
                ballList.Add(objectManager.MakeObj(ObjectNames.soccerBall));
                ballList[i].transform.position = transform.position;
                ballList[i].GetComponent<SoccerBall>().Initialize(curWeapons[ObjectNames.soccerBall]);
            }
            
            yield return soccerBallSec;
            foreach(GameObject obj in ballList)
                obj.SetActive(false);
        }
    }

    IEnumerator Shuriken(float cooltime)
    {
        shurikenSec = new WaitForSeconds(cooltime);

        while (!GameManager.IsPaused)
        {
            for (int i = 0; i < curWeapons[ObjectNames.shuriken].WeaponProjectileCount; i++)
            {
                GameObject shk = objectManager.MakeObj(ObjectNames.shuriken);
                shk.transform.position = transform.position;
                shk.GetComponent<Shuriken>().Initialize(curWeapons[ObjectNames.shuriken]);
            }

            yield return shurikenSec;
        }
    }

    IEnumerator Defender(float cooltime)
    {
        defenderSec = new WaitForSeconds(cooltime);

        while (!GameManager.IsPaused)
        {
            float rotateOffset = 360 / curWeapons[ObjectNames.defender].WeaponProjectileCount;

            for (int i = 0; i < curWeapons[ObjectNames.defender].WeaponProjectileCount; i++)
            {
                GameObject def = objectManager.MakeObj(ObjectNames.defender);
                def.transform.position = transform.position + (Vector3.up * 2);
                def.transform.RotateAround(transform.position, Vector3.forward, rotateOffset * i);
                def.GetComponent<Defender>().Initialize(curWeapons[ObjectNames.defender]);
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
