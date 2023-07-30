using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] GameObject enhancementSet;
    [SerializeField] GameObject exitSet;

    //캐릭터 선택 화면 관련
    [Header("캐릭터 선택")]
    int charId;
    [SerializeField] GameObject characterSelectSet;
    List<PlayerInfo> playerInfoList;
    [SerializeField] Image[] playerImgs;
    [SerializeField] TextMeshProUGUI[] playerNames;
    [SerializeField] Image[] playerWeaponImgs;

    //스테이지 선택 화면 관련
    [Header("스테이지 선택")]
    int stageId;
    [SerializeField] GameObject stageSelectSet;

    //골드, 구매 관련
    [Header("골드 / 구매")]
    [SerializeField] GameObject purchaseFailSet;
    [SerializeField] TextMeshProUGUI goldText;

    //강화 화면 관련
    [Header("강화 화면")]
    [SerializeField] EnhanceSlot[] enhanceSlots;
    [SerializeField] TextMeshProUGUI[] enhanceValueTexts;
    Dictionary<int, EnhancementData> enhanceDic;

    int curEnhanceId, curEnhanceIdx;
    [SerializeField] Image enhanceIcon;
    [SerializeField] TextMeshProUGUI enhanceNameText;
    [SerializeField] TextMeshProUGUI enhanceDescText;
    [SerializeField] TextMeshProUGUI enhancePriceText;
    [SerializeField] GameObject enhanceBtn;

    // 치트 관련
    // 배포 시 반드시 삭제 요망
    [SerializeField] GameObject cheatSet;

    void Awake()
    {
        charId = 0;
        stageId = 0;
        curEnhanceIdx = 0;

        playerInfoList = new List<PlayerInfo>();
        enhanceDic = new Dictionary<int, EnhancementData>();

        ReadData();
    }

    //csv 파일에서 데이터 읽어오기
    void ReadData()
    {
        //강화 데이터
        TextAsset enhanceDatas = Resources.Load("EnhancementDatas") as TextAsset;
        StringReader enhanceDataReader = new StringReader(enhanceDatas.text);

        while (enhanceDataReader != null)
        {
            string line = enhanceDataReader.ReadLine();
            if (line == null) break;

            line = enhanceDataReader.ReadLine();
            while (line.Length > 1)
            {
                string[] datas = line.Split(',');
                int id = int.Parse(datas[0]);

                //0: id, 1: spriteId, 2: name, 3: desc, 4: type, 5: val, 6: price
                EnhancementData e_data = new EnhancementData(id, int.Parse(datas[1]), datas[2], datas[3], int.Parse(datas[4]), float.Parse(datas[5]), int.Parse(datas[6]));
                enhanceDic.Add(id, e_data);

                line = enhanceDataReader.ReadLine();
                if (line == null) break;
            }
        }
        enhanceDataReader.Close();

        //플레이어블 캐릭터 데이터
        TextAsset playerInfos = Resources.Load("PlayerInfos") as TextAsset;
        StringReader playerInfoReader = new StringReader(playerInfos.text);

        while (playerInfoReader != null)
        {
            string line = playerInfoReader.ReadLine();
            if (line == null) break;

            line = playerInfoReader.ReadLine();
            while (line.Length > 1)
            {
                string[] datas = line.Split(',');
                //0: charId, 1: charName, 2: weaponId
                PlayerInfo info = new PlayerInfo(int.Parse(datas[0]), datas[1], int.Parse(datas[2]));
                playerInfoList.Add(info);

                line = playerInfoReader.ReadLine();
                if (line == null) break;
            }
        }
        playerInfoReader.Close();
    }

    void Start()
    {
        UpdateGold();
        SetEnhanceIds();
        UpdateCharacterInfo();
        UpdateEnhanceInfo(enhanceSlots[0].EnhanceId);
    }

    //캐릭터 선택창 관련
    void UpdateCharacterInfo()
    {
        for(int i = 0; i < playerInfoList.Count; i++)
        {
            playerNames[i].text = playerInfoList[i].charName;

            playerImgs[i].sprite = SpriteManager.getSprite(playerInfoList[i].charId);
            playerWeaponImgs[i].sprite = SpriteManager.getSprite(playerInfoList[i].weaponId);
        }
    }

    //게임 시작 버튼 관련
    //모드 선택
    public void Btn_GameStart()
    {
        SoundManager.playSfx(SoundManager.Sfx.btnClick);
        UpdateCharacterInfo();
        characterSelectSet.SetActive(true);
    }
    
    //캐릭터 선택
    public void Btn_characterSelect()
    {
        if (charId == 0) return;

        SoundManager.playSfx(SoundManager.Sfx.btnClick);
        characterSelectSet.SetActive(false);

        stageSelectSet.SetActive(true);
    }

    public void Btn_Character(int idx)
    {
        charId = playerInfoList[idx].charId;
    }

    public void Btn_characterSelectExit()
    {
        SoundManager.playSfx(SoundManager.Sfx.btnClick);
        characterSelectSet.SetActive(false);
        charId = 0;
    }

    //스테이지 선택
    public void Btn_StageSelectExit()
    {
        SoundManager.playSfx(SoundManager.Sfx.btnClick);
        stageSelectSet.SetActive(false);
        charId = 0;
        stageId = 0;
    }

    public void Btn_Stage(int id)
    {
        stageId = id;
    }

    //현재 강화 수치들을 PlayerStatus 클래스로 저장하여 PlayerStatusManager에 전달
    //스테이지 시작 시 해당 데이터를 Player에서 불러와 사용
    public void Btn_StageEnter()
    {
        if (stageId == 0) return;

        SoundManager.playSfx(SoundManager.Sfx.btnClick);
        //플레이어 스탯 전달
        //0: atkPower, 1:atkScale, 2:projSpeed, 3: cooltime, 4: projCount, 5: atkRemainTime, 6:playerHealth, 7: playerDef, 8: playerMoveSpeed
        PlayerStatus status = new PlayerStatus(
            enhanceDic[Prefers.Instance.AtkPower].enhanceVal,
            enhanceDic[Prefers.Instance.AtkScale].enhanceVal,
            enhanceDic[Prefers.Instance.ProjSpeed].enhanceVal,
            enhanceDic[Prefers.Instance.Cooltime].enhanceVal,
            (int)enhanceDic[Prefers.Instance.ProjCount].enhanceVal,
            enhanceDic[Prefers.Instance.AtkRemainTime].enhanceVal,
            enhanceDic[Prefers.Instance.PlayerHealth].enhanceVal,
            (int)enhanceDic[Prefers.Instance.Playerdef].enhanceVal,
            enhanceDic[Prefers.Instance.PlayerMoveSpeed].enhanceVal);
        PlayerStatusManager.setStatus(status);
        PlayerStatusManager.setCharacterId(charId);

        LoadingSceneManager.Inst.EnterStage(stageId);
    }

    //골드 UI 갱신
    public void UpdateGold()
    {
        goldText.text = string.Format("{0:n0}", GoldManager.Instance.Gold);
    }

    //강화 버튼 관련
    void SetEnhanceIds()
    {
        int[] ids = Prefers.Instance.GetPrefers();
        for(int i = 0; i < enhanceSlots.Length; i++)
        {
            if (enhanceDic.ContainsKey(ids[i] + 1))
                enhanceSlots[i].SetEnhanceId(ids[i] + 1);
            else
                enhanceSlots[i].SetEnhanceId(ids[i]);
        }
    }

    void UpdateEnhanceInfo(int id)
    {
        enhanceIcon.sprite = SpriteManager.getSprite(enhanceDic[id].spriteId);
        enhanceNameText.text = enhanceDic[id].nameText;
        enhanceDescText.text = enhanceDic[id].descText;
        enhancePriceText.text = enhanceDic[id].price == 0 ? "Max" : string.Format("{0:n0}", enhanceDic[id].price);
        enhanceBtn.SetActive(enhanceDic[id].price > 0);

        int offset;
        for (int i = 0; i < enhanceValueTexts.Length; i++)
        {
            offset = i == curEnhanceIdx ? 0 : 1;
            float val = Mathf.Round(enhanceDic[enhanceSlots[i].EnhanceId - offset].enhanceVal * 100);
            int idx = enhanceSlots[i].EnhanceId / 100;
            //4: 쿨타임 감소, 5: 투사체 수, 8: 방어력
            if (idx == 4)
                enhanceValueTexts[i].text = "- " + (100 - val) + "%";
            else if (idx == 5 || idx == 8)
                enhanceValueTexts[i].text = "+ " + val / 100;
            else
                enhanceValueTexts[i].text = "+ " + (val - 100) + "%";

            enhanceValueTexts[i].color = Color.white;
        }
        enhanceValueTexts[curEnhanceIdx].color = Color.green;
    }

    public void Btn_EnhancementOpen()
    {
        SoundManager.playSfx(SoundManager.Sfx.btnClick);
        if (enhancementSet.activeSelf)
        {
            enhancementSet.SetActive(false);
            UpdateEnhanceInfo(enhanceSlots[0].EnhanceId);
        }
        else
            enhancementSet.SetActive(true);
    }

    public void Btn_EnhanceSelect(int idx)
    {
        curEnhanceIdx = idx;
        curEnhanceId = enhanceSlots[idx].EnhanceId;
        UpdateEnhanceInfo(curEnhanceId);
    }

    public void Btn_Enhance()
    {
        //최대 레벨일 경우 스킵
        if (curEnhanceId == 0) return;

        //구매 실패 처리
        if (GoldManager.Instance.Purchase(enhanceDic[curEnhanceId].price) == false)
        {
            SoundManager.playSfx(SoundManager.Sfx.btnClick);
            purchaseFailSet.SetActive(true);
            return;
        }

        //UI, Prefs 변수 갱신
        SoundManager.playSfx(SoundManager.Sfx.statUpgrade);
        UpdateGold();
        Prefers.Instance.UpdatePref(enhanceDic[curEnhanceId].nameText);
        
        enhanceSlots[curEnhanceIdx].SetEnhanceId(++curEnhanceId);
        Btn_EnhanceSelect(curEnhanceIdx);
    }

    public void Btn_PurchaseFail()
    {
        SoundManager.playSfx(SoundManager.Sfx.btnClick);
        purchaseFailSet.SetActive(false);
    }

    //종료 버튼 관련
    public void Btn_Exit()
    {
        SoundManager.playSfx(SoundManager.Sfx.btnClick);
        if (exitSet.activeSelf)
            exitSet.SetActive(false);
        else
            exitSet.SetActive(true);
    }

    public void Btn_ExitYes()
    {
#if UNITY_EDITOR
        Prefers.Instance.UpdateAllPref();
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }

    //종료 시 Prefers 저장
    void OnApplicationQuit()
    {
        Prefers.Instance.UpdateAllPref();
    }

    ///////////////////////////////////
    //치트 관련
    //빌드 전 반드시 삭제 요망

    public void Cheat_On()
    {
        cheatSet.SetActive(!cheatSet.activeSelf);
    }

    public void Cheat_PlusGold(int plus)
    {
        GoldManager.Instance.PlusGold(plus);
        UpdateGold();
    }

    public void Cheat_RemoveGold()
    {
        GoldManager.Instance.Purchase(GoldManager.Instance.Gold);
        UpdateGold();
    }

    public void Cheat_EnhancementReset()
    {
        Prefers.Instance.Reset();
    }
}