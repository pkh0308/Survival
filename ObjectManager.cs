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
    [Header("플레이어 캐릭터")]
    int playerCharacterId;
    Player player;
    [SerializeField] GameObject kkurugiPrefab;
    [SerializeField] GameObject ninjaPrefab;
    [SerializeField] GameObject knightPrefab;
    [SerializeField] GameObject farmerPrefab;
    [SerializeField] GameObject hunterPrefab;
    [SerializeField] GameObject monkPrefab;
    Dictionary<int, CharacterData> characterDic;

    //무기 투사체
    [Header("무기 투사체")]
    [SerializeField] GameObject soccerBallPrefab;
    GameObject[] soccerBall;
    [SerializeField] GameObject shurikenPrefab;
    GameObject[] shuriken;
    [SerializeField] GameObject defenderPrefab;
    GameObject[] defender;
    [SerializeField] GameObject missilePrefab;
    GameObject[] missile;
    [SerializeField] GameObject thunderPrefab;
    GameObject[] thunder;
    [SerializeField] GameObject explodeMinePrefab;
    GameObject[] explodeMine;

    //업그레이드 무기 투사체
    [Header("업그레이드 무기 투사체")]
    [SerializeField] GameObject quantumBallPrefab;
    GameObject[] quantumBall;
    [SerializeField] GameObject shadowEdgePrefab;
    GameObject[] shadowEdge;
    [SerializeField] GameObject guardianPrefab;
    GameObject[] guardian;
    [SerializeField] GameObject sharkMissilePrefab;
    GameObject[] sharkMissile;
    [SerializeField] GameObject judgementPrefab;
    GameObject[] judgement;
    [SerializeField] GameObject hellfireMinePrefab;
    GameObject[] hellfireMine;

    [Header("폭발 판정")]
    [SerializeField] GameObject explosionPrefab;
    GameObject[] explosion;

    //몬스터
    [Header("몬스터")]
    [SerializeField] GameObject zombiePrefab;
    [SerializeField] GameObject zombie_UniquePrefab;
    [SerializeField] GameObject salesmanPrefab;
    [SerializeField] GameObject monsterTreePrefab;
    GameObject[] zombie;
    GameObject[] zombie_Unique;
    GameObject[] salesman;
    GameObject[] monsterTree;

    [Header("몬스터 투사체")]
    [SerializeField] GameObject stonePrefab;
    [SerializeField] GameObject phonePrefab;
    GameObject[] stone;
    GameObject[] phone;

    //경험치 젬
    [Header("경험치 젬")]
    [SerializeField] GameObject expLowPrefab;
    [SerializeField] GameObject expMiddlePrefab;
    [SerializeField] GameObject expHighPrefab;
    GameObject[] expGemLow;
    GameObject[] expGemMiddle;
    GameObject[] expGemHigh;

    //아이템
    [Header("아이템")]
    [SerializeField] GameObject gold_10Prefab;
    [SerializeField] GameObject gold_50Prefab;
    [SerializeField] GameObject gold_100Prefab;
    [SerializeField] GameObject meat_50Prefab;
    [SerializeField] GameObject magnetPrefab;
    [SerializeField] GameObject bombPrefab;
    GameObject[] gold_10;
    GameObject[] gold_50;
    GameObject[] gold_100;
    GameObject[] meat_50;
    GameObject[] magnet;
    GameObject[] bomb;

    //아이템 박스
    [Header("아이템 박스")]
    [SerializeField] GameObject itemBoxPrefab;
    GameObject[] itemBox;

    //아이템 박스
    [Header("보물 상자")]
    [SerializeField] GameObject treasureBoxPrefab;
    GameObject[] treasureBox;

    //기타
    [Header("기타")]
    [SerializeField] GameObject bossAreaPrefab;
    GameObject[] bossArea;

    //UI
    [Header("UI")]
    [SerializeField] Canvas minorCanvas;
    [SerializeField] TextMeshProUGUI textPrefab;
    TextMeshProUGUI[] texts;

    GameObject[] targetPool;

    public static Func<int, GameObject> dropExp;
    public static Func<int, GameObject> makeWeaponProj;
    public static Func<int, GameObject> makeEnemy;
    public static Func<int, GameObject> makeEnemyBullet;
    public static Func<int, GameObject> makeItem;
    public static Func<int, GameObject> makeObj;

    void Awake()
    {
        playerCharacterId = PlayerStatusManager.getCharacterId();
        characterDic = new Dictionary<int, CharacterData>();

        dropExp = (a) => { return DropExp(a); };
        makeWeaponProj = (id) => { return MakeWeaponProj(id); };
        makeEnemy = (id) => { return MakeEnemy(id); };
        makeEnemyBullet = (id) => { return MakeEnemyBullet(id); };
        makeItem = (id) => { return MakeItem(id); };
        makeObj = (id) => { return MakeObj(id); };

        //무기 투사체
        soccerBall = new GameObject[30];
        shuriken = new GameObject[30];
        defender = new GameObject[10];
        missile = new GameObject[30];
        thunder = new GameObject[30];
        explodeMine = new GameObject[30];

        //업그레이드 무기 투사체
        quantumBall = new GameObject[30];
        shadowEdge = new GameObject[30];
        guardian = new GameObject[10];
        sharkMissile = new GameObject[30];
        judgement = new GameObject[30];
        hellfireMine = new GameObject[30];

        //폭발 판정 콜라이더(PlayerBullet)
        explosion = new GameObject[100];

        //몬스터
        zombie = new GameObject[300];
        zombie_Unique = new GameObject[10];
        salesman = new GameObject[300];
        monsterTree = new GameObject[3];

        //몬스터 투사체
        stone = new GameObject[100];
        phone = new GameObject[100];

        //경험치 젬
        expGemLow = new GameObject[1000];
        expGemMiddle = new GameObject[1000];
        expGemHigh = new GameObject[1000];

        //아이템
        gold_10 = new GameObject[1000];
        gold_50 = new GameObject[1000];
        gold_100 = new GameObject[1000];
        meat_50 = new GameObject[1000];
        magnet = new GameObject[1000];
        bomb = new GameObject[1000];

        //아이템 박스
        itemBox = new GameObject[300];

        //보물 상자
        treasureBox = new GameObject[50];

        //기타
        bossArea = new GameObject[5];

        //UI_텍스트
        texts = new TextMeshProUGUI[1000];

        ReadCharacterData();
    }

    void Start()
    {
        LoadingSceneManager.Inst.SetActiveSceneToPlayerScene();
        StartCharacter(playerCharacterId);
        LoadingSceneManager.Inst.SetActiveSceneToCurStage();
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
        //몬스터
        for (int idx = 0; idx < zombie.Length; idx++)
        {
            zombie[idx] = Instantiate(zombiePrefab);
            zombie[idx].SetActive(false);
        }
        for (int idx = 0; idx < zombie_Unique.Length; idx++)
        {
            zombie_Unique[idx] = Instantiate(zombie_UniquePrefab);
            zombie_Unique[idx].SetActive(false);
        }
        for (int idx = 0; idx < salesman.Length; idx++)
        {
            salesman[idx] = Instantiate(salesmanPrefab);
            salesman[idx].SetActive(false);
        }
        for (int idx = 0; idx < monsterTree.Length; idx++)
        {
            monsterTree[idx] = Instantiate(monsterTreePrefab);
            monsterTree[idx].SetActive(false);
        }
        //몬스터 투사체
        for (int idx = 0; idx < stone.Length; idx++)
        {
            stone[idx] = Instantiate(stonePrefab);
            stone[idx].SetActive(false);
        }
        for (int idx = 0; idx < phone.Length; idx++)
        {
            phone[idx] = Instantiate(phonePrefab);
            phone[idx].SetActive(false);
        }

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
        for (int idx = 0; idx < missile.Length; idx++)
        {
            missile[idx] = Instantiate(missilePrefab);
            missile[idx].SetActive(false);
        }
        for (int idx = 0; idx < thunder.Length; idx++)
        {
            thunder[idx] = Instantiate(thunderPrefab);
            thunder[idx].SetActive(false);
        }
        for (int idx = 0; idx < explodeMine.Length; idx++)
        {
            explodeMine[idx] = Instantiate(explodeMinePrefab);
            explodeMine[idx].SetActive(false);
        }
        //업그레이드 무기 투사체
        for (int idx = 0; idx < quantumBall.Length; idx++)
        {
            quantumBall[idx] = Instantiate(quantumBallPrefab);
            quantumBall[idx].SetActive(false);
        }
        for (int idx = 0; idx < shadowEdge.Length; idx++)
        {
            shadowEdge[idx] = Instantiate(shadowEdgePrefab);
            shadowEdge[idx].SetActive(false);
        }
        for (int idx = 0; idx < guardian.Length; idx++)
        {
            guardian[idx] = Instantiate(guardianPrefab, player.transform);
            guardian[idx].SetActive(false);
        }
        for (int idx = 0; idx < sharkMissile.Length; idx++)
        {
            sharkMissile[idx] = Instantiate(sharkMissilePrefab);
            sharkMissile[idx].SetActive(false);
        }
        for (int idx = 0; idx < judgement.Length; idx++)
        {
            judgement[idx] = Instantiate(judgementPrefab);
            judgement[idx].SetActive(false);
        }
        for (int idx = 0; idx < hellfireMine.Length; idx++)
        {
            hellfireMine[idx] = Instantiate(hellfireMinePrefab);
            hellfireMine[idx].SetActive(false);
        }

        //폭발 판정 콜라이더(explosion)
        for (int idx = 0; idx < explosion.Length; idx++)
        {
            explosion[idx] = Instantiate(explosionPrefab);
            explosion[idx].SetActive(false);
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

        //아이템
        for (int idx = 0; idx < gold_10.Length; idx++)
        {
            gold_10[idx] = Instantiate(gold_10Prefab);
            gold_10[idx].SetActive(false);
        }
        for (int idx = 0; idx < gold_50.Length; idx++)
        {
            gold_50[idx] = Instantiate(gold_50Prefab);
            gold_50[idx].SetActive(false);
        }
        for (int idx = 0; idx < gold_100.Length; idx++)
        {
            gold_100[idx] = Instantiate(gold_100Prefab);
            gold_100[idx].SetActive(false);
        }
        for (int idx = 0; idx < meat_50.Length; idx++)
        {
            meat_50[idx] = Instantiate(meat_50Prefab);
            meat_50[idx].SetActive(false);
        }
        for (int idx = 0; idx < magnet.Length; idx++)
        {
            magnet[idx] = Instantiate(magnetPrefab);
            magnet[idx].SetActive(false);
        }
        for (int idx = 0; idx < bomb.Length; idx++)
        {
            bomb[idx] = Instantiate(bombPrefab);
            bomb[idx].SetActive(false);
        }

        //아이템 박스
        for (int idx = 0; idx < itemBox.Length; idx++)
        {
            itemBox[idx] = Instantiate(itemBoxPrefab);
            itemBox[idx].SetActive(false);
        }
        //보물 상자
        for (int idx = 0; idx < treasureBox.Length; idx++)
        {
            treasureBox[idx] = Instantiate(treasureBoxPrefab);
            treasureBox[idx].SetActive(false);
        }
        //기타
        for (int idx = 0; idx < bossArea.Length; idx++)
        {
            bossArea[idx] = Instantiate(bossAreaPrefab);
            bossArea[idx].SetActive(false);
        }

        //텍스트
        for (int idx = 0; idx < texts.Length; idx++)
        {
            texts[idx] = Instantiate(textPrefab, minorCanvas.transform);
            texts[idx].gameObject.SetActive(false);
        }
    }

    //플레이어 캐릭터 초기화
    public void StartCharacter(int id)
    {
        GameObject target;
        switch (id)
        {
            case ObjectNames.kkurugi:
                target = Instantiate(kkurugiPrefab);
                break;
            case ObjectNames.ninja:
                target = Instantiate(ninjaPrefab);
                break;
            case ObjectNames.knight:
                target = Instantiate(knightPrefab);
                break;
            case ObjectNames.farmer:
                target = Instantiate(farmerPrefab);
                break;
            case ObjectNames.hunter:
                target = Instantiate(hunterPrefab);
                break;
            case ObjectNames.monk:
                target = Instantiate(monkPrefab);
                break;
            default:
                target = null;
                break;
        }

        player = target.GetComponent<Player>();
        player.Initialize(gameManager, uiManager, characterDic[id]);
        uiManager.GetWeaponLogic(target.GetComponent<Weapons>());
        target.GetComponent<Weapons>().SetObjectManager(this);
    }

    //무기 투사체 생성
    public GameObject MakeWeaponProj(int id)
    {
        switch (id)
        {
            //무기 투사체
            case ObjectNames.soccerBall:
                targetPool = soccerBall;
                break;
            case ObjectNames.shuriken:
                targetPool = shuriken;
                break;
            case ObjectNames.defender:
                targetPool = defender;
                break;
            case ObjectNames.missile:
                targetPool = missile;
                break;
            case ObjectNames.thunder:
                targetPool = thunder;
                break;
            case ObjectNames.explodeMine:
                targetPool = explodeMine;
                break;
            //업그레이드 무기 투사체
            case ObjectNames.quantumBall:
                targetPool = quantumBall;
                break;
            case ObjectNames.shadowEdge:
                targetPool = shadowEdge;
                break;
            case ObjectNames.guardian:
                targetPool = guardian;
                break;
            case ObjectNames.sharkMissile:
                targetPool = sharkMissile;
                break;
            case ObjectNames.judgement:
                targetPool = judgement;
                break;
            case ObjectNames.hellfireMine:
                targetPool = hellfireMine;
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

    //몬스터 생성
    public GameObject MakeEnemy(int id)
    {
        switch (id)
        {
            //몬스터
            case ObjectNames.zombie:
                targetPool = zombie;
                break;
            case ObjectNames.zombie_Unique:
                targetPool = zombie_Unique;
                break;
            case ObjectNames.salesman:
                targetPool = salesman;
                break;
            case ObjectNames.monsterTree:
                targetPool = monsterTree;
                break;
            //몬스터 투사체
            case ObjectNames.stone:
                targetPool = stone;
                break;
            case ObjectNames.phone:
                targetPool = phone;
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

    //몬스터 투사체 생성
    public GameObject MakeEnemyBullet(int id)
    {
        switch (id)
        {
            //몬스터 투사체
            case ObjectNames.stone:
                targetPool = stone;
                break;
            case ObjectNames.phone:
                targetPool = phone;
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

    //아이템 생성
    public GameObject MakeItem(int id)
    {
        switch (id)
        {
            //아이템
            case ObjectNames.gold_10:
                targetPool = gold_10;
                break;
            case ObjectNames.gold_50:
                targetPool = gold_50;
                break;
            case ObjectNames.gold_100:
                targetPool = gold_100;
                break;
            case ObjectNames.meat_50:
                targetPool = meat_50;
                break;
            case ObjectNames.magnet:
                targetPool = magnet;
                break;
            case ObjectNames.bomb:
                targetPool = bomb;
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

    //기타 오브젝트 생성
    public GameObject MakeObj(int id)
    {
        switch (id)
        {
            //폭발 판정 콜라이더
            case ObjectNames.explosion:
                targetPool = explosion;
                break;
            //아이템 박스
            case ObjectNames.itemBox:
                targetPool = itemBox;
                break;
            //보물 상자
            case ObjectNames.treasureBox:
                targetPool = treasureBox;
                break;
            //보스 전투 영역
            case ObjectNames.bossArea:
                targetPool = bossArea;
                break;
        }

        for (int idx = 0; idx < targetPool.Length; idx++)
        {
            if (!targetPool[idx].activeSelf)
            {
                if(targetPool != explosion) //explosion은 비활성 상태로 전달
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