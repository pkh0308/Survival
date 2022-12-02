using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectManager : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] UiManager uiManager;

    //플레이어 캐릭터
    int playerCharacterId;
    Player player;
    [SerializeField] GameObject player_1101Prefab;
    [SerializeField] GameObject player_1201Prefab;
    [SerializeField] GameObject player_1301Prefab;
    Dictionary<int, CharacterData> characterDic;

    //무기 투사체
    [SerializeField] GameObject soccerBallPrefab;
    GameObject[] soccerBall;
    [SerializeField] GameObject shurikenPrefab;
    GameObject[] shuriken;
    [SerializeField] GameObject defenderPrefab;
    GameObject[] defender;

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

    //UI
    [SerializeField] Canvas minorCanvas;
    [SerializeField] TextMeshProUGUI textPrefab;
    TextMeshProUGUI[] texts;

    GameObject[] targetPool;

    public static Func<int, GameObject> dropExp;

    void Awake()
    {
        playerCharacterId = PlayerStatusManager.getCharacterId();
        characterDic = new Dictionary<int, CharacterData>();

        dropExp = (a) => { return DropExp(a); };

        soccerBall = new GameObject[30];
        shuriken = new GameObject[30];
        defender = new GameObject[10];

        enemyMelee = new GameObject[100];

        expGemLow = new GameObject[1000];
        expGemMiddle = new GameObject[1000];
        expGemHigh = new GameObject[1000];

        texts = new TextMeshProUGUI[1000];

        ReadCharacterData();
    }

    void Start()
    {
        LoadingSceneManager.setActiveSceneToPlayerScene();
        StartCharacter(playerCharacterId);
        LoadingSceneManager.setActiveSceneToCurStage();
        Generate();
    }

    void ReadCharacterData()
    {
        //csv 파일에서 데이터 읽어오기
        TextAsset characterData = Resources.Load("CharacterDatas") as TextAsset;
        StringReader characterDataReader = new StringReader(characterData.text);

        while (characterDataReader != null)
        {
            string line = characterDataReader.ReadLine();
            if (line == null) break;

            line = characterDataReader.ReadLine();
            while (line.Length > 1)
            {
                string[] datas = line.Split(',');
                int id = int.Parse(datas[0]);

                // 0: id, 1: atk, 2: scale, 3:projSpeed, 4:cooltime, 5:count, 6: remainTime, 7: health, 8:def, 9:moveSpeed
                CharacterData c_data = new CharacterData(id, float.Parse(datas[1]), float.Parse(datas[2]), float.Parse(datas[3]), float.Parse(datas[4]), 
                                                         int.Parse(datas[5]), float.Parse(datas[6]), int.Parse(datas[7]), int.Parse(datas[8]), float.Parse(datas[9]));
                characterDic.Add(id, c_data);

                line = characterDataReader.ReadLine();
                if (line == null) break;
            }
        }
        characterDataReader.Close();
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
        for (int idx = 0; idx < shuriken.Length; idx++)
        {
            shuriken[idx] = Instantiate(shurikenPrefab);
            shuriken[idx].SetActive(false);
        }
        for (int idx = 0; idx < defender.Length; idx++)
        {
            defender[idx] = Instantiate(defenderPrefab, player.transform);
            defender[idx].SetActive(false);
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

        //텍스트
        for (int idx = 0; idx < texts.Length; idx++)
        {
            texts[idx] = Instantiate(textPrefab, minorCanvas.transform);
            texts[idx].gameObject.SetActive(false);
        }
    }

    public GameObject StartCharacter(int id)
    {
        GameObject target;
        switch(id)
        {
            case ObjectNames.kkurugi:
                target = Instantiate(player_1101Prefab);
                break;
            case ObjectNames.ninja:
                target = Instantiate(player_1201Prefab);
                break;
            case ObjectNames.knight:
                target = Instantiate(player_1301Prefab);
                break;
            default:
                return null;
        }

        player = target.GetComponent<Player>();
        player.Initialize(gameManager, uiManager, characterDic[id]);
        uiManager.GetWeaponLogic(target.GetComponent<Weapons>());
        target.GetComponent<Weapons>().SetObjectManager(this);
        return target;
    }

    public GameObject MakeObj(int id)
    {
        switch (id)
        {
            case ObjectNames.soccerBall:
                targetPool = soccerBall;
                break;
            case ObjectNames.shuriken:
                targetPool = shuriken;
                break;
            case ObjectNames.defender:
                targetPool = defender;
                break;
            case ObjectNames.enemyMelee:
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
    
    public TextMeshProUGUI MakeText()
    {
        for (int idx = 0; idx < texts.Length; idx++)
        {
            if (!texts[idx].gameObject.activeSelf)
            {
                texts[idx].gameObject.SetActive(true);
                return texts[idx];
            }
        }
        return null;
    }
}
