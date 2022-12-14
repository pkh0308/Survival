using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] LobbySoundManager soundManager;
    [SerializeField] ImageContainer imgContainer;

    [SerializeField] GameObject enhancementSet;
    [SerializeField] GameObject exitSet;

    //캐릭터 선택 화면 관련
    int charId;
    [SerializeField] GameObject characterSelectSet;

    //스테이지 선택 화면 관련
    int stageId;
    [SerializeField] GameObject stageSelectSet;

    //골드, 구매 관련
    [SerializeField] GameObject purchaseFailSet;
    [SerializeField] TextMeshProUGUI goldText;

    //강화 화면 관련
    [SerializeField] EnhanceSlot[] enhanceSlots;
    [SerializeField] TextMeshProUGUI[] enhanceValueTexts;
    Dictionary<int, EnhancementData> enhanceDic;

    int curEnhanceId, curEnhanceIdx;
    [SerializeField] Image enhanceIcon;
    [SerializeField] TextMeshProUGUI enhanceNameText;
    [SerializeField] TextMeshProUGUI enhanceDescText;
    [SerializeField] TextMeshProUGUI enhancePriceText;
    [SerializeField] GameObject enhanceBtn;

    //치트 관련
    //빌드 전 반드시 삭제 요망
    [SerializeField] GameObject cheatSet;

    void Awake()
    {
        charId = 0;
        stageId = 0;
        curEnhanceIdx = 0;

        enhanceDic = new Dictionary<int, EnhancementData>();
        ReadEnhancementData();
    }

    void ReadEnhancementData()
    {
        //csv 파일에서 데이터 읽어오기
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
    }

    void Start()
    {
        UpdateGold();
        SetEnhanceIds();
        UpdateEnhanceInfo(enhanceSlots[0].EnhanceId);
    }

    //게임 시작 버튼 관련
    public void Btn_GameStart()
    {
        soundManager.PlaySfx((int)LobbySoundManager.LobbySfx.btnClick);
        characterSelectSet.SetActive(true);
    }

    public void Btn_characterSelect()
    {
        if (charId == 0) return;

        soundManager.PlaySfx((int)LobbySoundManager.LobbySfx.btnClick);
        characterSelectSet.SetActive(false);
        stageSelectSet.SetActive(true);
    }

    public void Btn_Character(int id)
    {
        charId = id;
    }

    public void Btn_characterSelectExit()
    {
        soundManager.PlaySfx((int)LobbySoundManager.LobbySfx.btnClick);
        characterSelectSet.SetActive(false);
        charId = 0;
    }

    public void Btn_StageSelectExit()
    {
        soundManager.PlaySfx((int)LobbySoundManager.LobbySfx.btnClick);
        stageSelectSet.SetActive(false);
        charId = 0;
        stageId = 0;
    }

    public void Btn_Stage(int id)
    {
        stageId = id;
    }

    public void Btn_StageEnter()
    {
        if (stageId == 0) return;

        soundManager.PlaySfx((int)LobbySoundManager.LobbySfx.btnClick);
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

        LoadingSceneManager.enterStage(stageId);
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
        enhanceIcon.sprite = imgContainer.GetSprite(enhanceDic[id].spriteId);
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
        soundManager.PlaySfx((int)LobbySoundManager.LobbySfx.btnClick);
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
            soundManager.PlaySfx((int)LobbySoundManager.LobbySfx.btnClick);
            purchaseFailSet.SetActive(true);
            return;
        }

        //UI, Prefs 변수 갱신
        soundManager.PlaySfx((int)LobbySoundManager.LobbySfx.upgrade);
        UpdateGold();
        Prefers.Instance.UpdatePref(enhanceDic[curEnhanceId].nameText);
        
        enhanceSlots[curEnhanceIdx].SetEnhanceId(++curEnhanceId);
        Btn_EnhanceSelect(curEnhanceIdx);
    }

    public void Btn_PurchaseFail()
    {
        soundManager.PlaySfx((int)LobbySoundManager.LobbySfx.btnClick);
        purchaseFailSet.SetActive(false);
    }

    //종료 버튼 관련
    public void Btn_Exit()
    {
        soundManager.PlaySfx((int)LobbySoundManager.LobbySfx.btnClick);
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
