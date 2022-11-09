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

    //축구공
    Coroutine soccerBall;
    WaitForSeconds soccerBallSec;

    void Awake()
    {
        weaponDic = new Dictionary<int, WeaponData[]>();
        curWeapons = new Dictionary<int, WeaponData>();

        ReadData();
    }

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
                weaponDic.Add(id, new WeaponData[5]);
                
                for(int i = 0; i < 5; i++)
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

    void Start()
    {
        
    }

    public void GetWeapon(int id)
    {
        if (curWeapons.TryGetValue(id, out WeaponData w))
            curWeapons[id] = weaponDic[id][curWeapons[id].WeaponLevel + 1];
        else
            curWeapons.Add(id, weaponDic[id][0]);

        StartWeaponRoutine(id);
    }

    void StartWeaponRoutine(int id)
    {
        switch(id)
        {
            case 1001:
                if (soccerBall != null) StopCoroutine(soccerBall);
                soccerBall = StartCoroutine(SoccerBall(curWeapons[id].WeaponCooltime));
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
}
