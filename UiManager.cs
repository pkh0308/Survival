using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Random = UnityEngine.Random; // System.Random과 혼선 방지용

public class UiManager : MonoBehaviour
{
    [Header("컴포넌트 연결")]
    [SerializeField] GameManager gameManager;
    [SerializeField] ObjectManager objectManager;
    [SerializeField] StageSoundManager soundManager;
    Weapons weaponLogic;

    //타이머 관련
    [Header("타이머")]
    [SerializeField] TextMeshProUGUI timerText;
    int mm = 0;
    int ss = 0;

    //카운트 관련
    [Header("카운트")]
    [SerializeField] TextMeshProUGUI killCountText;
    [SerializeField] TextMeshProUGUI goldCountText;

    //일시정지 관련
    [Header("일시정지")]
    [SerializeField] GameObject pauseSet;
    [SerializeField] GameObject exitPopupSet;
    [SerializeField] Image[] weaponIcons;
    [SerializeField] Image[] accessoryIcons;
    [SerializeField] GameObject statisticsSet;
    [SerializeField] Image[] statisticsIcons;
    [SerializeField] Image[] statisticsBars;
    [SerializeField] TextMeshProUGUI[] statisticsTexts;
    [SerializeField] GameObject[] statisticsSlots;
    Dictionary<int, int> weaponIconDic;
    public static Action<int> updateWeaponAccList;

    //전투 관련
    [Header("전투")]
    [SerializeField] Image hpBarSet;
    [SerializeField] Image hpBar;
    Vector3 hpPos;
    Vector3 hpPosOffset;
    Vector3 hpBarScale;
    public static Action<int, Vector3> showDamage;

    //경험치 관련
    [Header("경험치")]
    [SerializeField] Image expBar;
    [SerializeField] TextMeshProUGUI levelText;
    Vector3 expBarScale;

    //무기 획득 관련
    [Header("무기 획득")]
    [SerializeField] GameObject weaponSelectSet;
    [SerializeField] GameObject[] weaponSelectSlots;
    Image[] weaponSelectSlots_Bg;

    DataForLevelUp[] levelupDatas;
    int curWeaponIdx;
    [SerializeField] Image[] weaponImages;
    [SerializeField] TextMeshProUGUI[] weaponName;
    [SerializeField] TextMeshProUGUI[] weaponDesc;
    [SerializeField] GameObject[] levelUpStarSets;
    [SerializeField] GameObject[] legandaryStars;
    [SerializeField] GameObject[] levelUpStars_0;
    [SerializeField] GameObject[] levelUpStars_1;
    [SerializeField] GameObject[] levelUpStars_2;
    List<GameObject[]> levelUpStars;
    

    //보물상자 획득 관련
    [Header("보물 상자")]
    [SerializeField] GameObject lotterySet;
    [SerializeField] GameObject lotteryStartBtn;
    [SerializeField] Image[] lotterySlots;
    [SerializeField] GameObject lotteryHighlightAnimation;
    [SerializeField] Image lotteryHighlight;
    [SerializeField] TextMeshProUGUI lotteryGoldText;
    [SerializeField] GameObject lotteryResultSet;
    [SerializeField] Image lotteryResultIcon;
    [SerializeField] TextMeshProUGUI lotteryResName;
    [SerializeField] TextMeshProUGUI lotteryResDesc;
    [SerializeField] GameObject[] lotteryResStars;
    [SerializeField] GameObject lotteryStarSet;
    [SerializeField] GameObject lotteryResStarForLegandary;
    [SerializeField] LotteryGold lotteryGold;
    Vector3 initialHighlightPos;
    DataForLevelUp lotteryTargetData;
    int lotteryTargetIdx;

    //스테이지 클리어 관련
    [Header("스테이지 클리어")]
    [SerializeField] GameObject stageClearSet;
    [SerializeField] TextMeshProUGUI killCountText_stageClear;
    [SerializeField] TextMeshProUGUI moneyCountText_stageClear;

    //게임 오버 관련
    [Header("게임 오버")]
    [SerializeField] GameObject gameOverSet;
    [SerializeField] TextMeshProUGUI killCountText_gameover;
    [SerializeField] TextMeshProUGUI moneyCountText_gameover;

    public void GetWeaponLogic(Weapons weaponLogic)
    {
        this.weaponLogic = weaponLogic;
    }

    void Awake()
    {
        showDamage = (dmg, positioon) => { StartCoroutine(ShowDamage(dmg, positioon)); };
        updateWeaponAccList = (id) => { UpdateWeaponAccList(id); };

        weaponIconDic = new Dictionary<int, int>();

        hpPosOffset = new Vector3(0, -60, 0);
        hpBarScale = Vector3.one;
        expBarScale = Vector3.one;

        weaponSelectSlots_Bg = new Image[weaponSelectSlots.Length];
        for (int i = 0; i < weaponSelectSlots.Length; i++)
            weaponSelectSlots_Bg[i] = weaponSelectSlots[i].GetComponent<Image>();

        levelUpStars = new List<GameObject[]>();
        levelUpStars.Add(levelUpStars_0);
        levelUpStars.Add(levelUpStars_1);
        levelUpStars.Add(levelUpStars_2);
    }

    void Start()
    {
        UpdateKillCount(0);
        UpdateGoldCount(0);
    }

    IEnumerator ShowDamage(int dmg, Vector3 pos)
    {
        TextMeshProUGUI dmgText = objectManager.MakeText();
        dmgText.text = string.Format("{0:n0}", dmg);
        dmgText.transform.position = pos;

        yield return new WaitForSeconds(1.0f);
        dmgText.gameObject.SetActive(false);
    }

    //플레이어가 FixedUpdate 주기로 움직이므로 체력바 위치도 FixedUpdate로 갱신
    void FixedUpdate()
    {
        HpBarPosUpdate();
    }

    void HpBarPosUpdate()
    {
        hpPos =  Camera.main.WorldToScreenPoint(Player.playerPos) + hpPosOffset;
        hpBarSet.rectTransform.position = hpPos;
    }

    //타이머 관련
    public void UpdateTimer(int seconds)
    {
        mm = seconds / 60;
        ss = seconds % 60;
        timerText.text = string.Format("{0:00}:{1:00}", mm, ss);
    }

    //일시정지 관련
    public bool Pause(bool pause)
    {
        if(exitPopupSet.activeSelf)
        {
            exitPopupSet.SetActive(false);
            return true;
        }
        pauseSet.SetActive(!pause);
        return pauseSet.activeSelf;
    }

    public void Btn_ExitStage()
    {
        exitPopupSet.SetActive(!exitPopupSet.activeSelf);
    }

    public void Btn_ExitYes()
    {
        LoadingSceneManager.exitStage();
    }

    public void Btn_Statics()
    {
        if (!statisticsSet.activeSelf)
            UpdateAccumulatedDmg();

        statisticsSet.SetActive(!statisticsSet.activeSelf);
    }

    //누적데미지 통계 갱신
    void UpdateAccumulatedDmg()
    {
        int total = Weapons.getTotalDmg();
        int[,] arr = Weapons.getAccumulatedDmg();

        for(int i = 0; i < arr.GetLength(0); i++)
        {
            statisticsBars[i].rectTransform.localScale = total == 0 ? Vector3.one : new Vector3((float)arr[i, 1] / total, 1, 1);
            statisticsIcons[i].sprite = SpriteContainer.getSprite(arr[i, 0]);
            statisticsTexts[i].text = string.Format("{0:n0}", arr[i, 1]);
            statisticsSlots[i].SetActive(true);
        }
        //남는 슬롯은 비활성화
        for (int i = arr.GetLength(0); i < 6; i++)
        {
            statisticsSlots[i].SetActive(false);
        }
    }

    //전투 관련
    public void UpdateHp(int cur, int max)
    {
        hpBarScale.x = (float)cur / max;
        hpBar.rectTransform.localScale = hpBarScale;
    }

    //카운트 관련
    public void UpdateKillCount(int count)
    {
        killCountText.text = string.Format("{0:n0}", count);
    }

    public void UpdateGoldCount(int count)
    {
        goldCountText.text = string.Format("{0:n0}", count);
    }

    //경험치 관련
    public void UpdateExp(int cur, int max)
    {
        expBarScale.x = cur > max ? 1 : (float)cur / max;
        expBar.rectTransform.localScale = expBarScale;
    }

    public void UpdateLevel(int level)
    {
        levelText.text = "Lv." + level;
    }

    //무기 획득 관련
    public void WeaponSelect()
    {
        //3개의 버튼에 랜덤 무기 노출
        levelupDatas = weaponLogic.GetRandomWeaponData().ToArray();
        for (int i = 0; i < levelupDatas.Length; i++)
        {
            weaponImages[i].sprite = SpriteContainer.getSprite(levelupDatas[i].id);
            weaponName[i].text = levelupDatas[i].name;
            weaponDesc[i].text = levelupDatas[i].description;
            //업그레이드 무기일 경우
            if(levelupDatas[i].id % 10 == 9)
            {
                weaponSelectSlots_Bg[i].sprite = SpriteContainer.getSprite(ObjectNames.weaponSelectSlotLegandary);
                levelUpStarSets[i].SetActive(false);
                legandaryStars[i].SetActive(true);
            }
            else
            {
                weaponSelectSlots_Bg[i].sprite = SpriteContainer.getSprite(ObjectNames.weaponSelectSlotNormal);

                for (int k = 0; k < levelupDatas[i].level; k++)
                    levelUpStars[i][k].SetActive(true);
                for (int k = levelupDatas[i].level; k < levelUpStars[i].Length; k++)
                    levelUpStars[i][k].SetActive(false);

                levelUpStarSets[i].SetActive(true);
                legandaryStars[i].SetActive(false);
            }

            weaponSelectSlots[i].SetActive(true);
        }
        //획득 또는 업그레이드 가능한 무기가 3개 미만일 경우
        for (int i = levelupDatas.Length; i < 3; i++)
        {
            weaponSelectSlots[i].SetActive(false);
        }

        weaponSelectSet.SetActive(true);
    }

    public void Btn_WeaponSelect(int idx)
    {
        curWeaponIdx = idx;
    }

    public void Btn_WeaponSelectComplete()
    {
        weaponLogic.GetWeapon(levelupDatas[curWeaponIdx].id);
        weaponSelectSet.SetActive(false);
        gameManager.PauseOff();

        weaponLogic.RestartWeapons();
        gameManager.LevelUp();
    }

    //무기, 악세사리 목록 업데이트
    public void UpdateWeaponAccList(int id)
    {
        if(id < 3000) //무기
        {
            if(id % 10 == 9) //업그레이드 무기일 경우
            {
                int beforeId = id / 10 * 10 + 1;
                weaponIcons[weaponIconDic[beforeId]].sprite = SpriteContainer.getSprite(id);
                return;
            }

            for (int i = 0; i < weaponIcons.Length; i++)
            {
                if (weaponIcons[i].gameObject.activeSelf)
                    continue;

                weaponIcons[i].sprite = SpriteContainer.getSprite(id);
                weaponIconDic.Add(id, i);
                weaponIcons[i].gameObject.SetActive(true);
                break;
            }
        }
        else //악세사리
        {
            for (int i = 0; i < accessoryIcons.Length; i++)
            {
                if (accessoryIcons[i].gameObject.activeSelf)
                    continue;

                accessoryIcons[i].sprite = SpriteContainer.getSprite(id);
                accessoryIcons[i].gameObject.SetActive(true);
                break;
            }
        }
    }

    //보물상자 획득
    public void ShowLottery()
    {
        //무기 정보 받아오기
        DataForLevelUp[] datas = weaponLogic.GetLotteryWeaponData(out int id);

        int idx;
        for(int i = 0; i < lotterySlots.Length; i++)
        {
            idx = Random.Range(0, datas.Length);
            lotterySlots[i].sprite = SpriteContainer.getSprite(datas[idx].id);
        }
        lotteryTargetIdx = Random.Range(0, lotterySlots.Length);
        lotteryTargetData = datas[lotteryTargetIdx];

        //업그레이드 가능한 무기가 있을 경우 무작위 슬롯 하나를 덮어씌우고,
        //lotteryTargetData를 업그레이드 무기로 고정
        if (id > 0) 
        {
            idx = Random.Range(0, lotterySlots.Length);
            lotterySlots[idx].sprite = SpriteContainer.getSprite(id);
            lotteryTargetIdx = idx;
            lotteryTargetData = datas[0];
        }
        
        lotterySet.SetActive(true);
        soundManager.PlayBgm((int)StageSoundManager.StageBgm.lotteryBgm);
    }

    public void Btn_StartLottery()
    {
        lotteryStartBtn.SetActive(false);
        StartCoroutine(Lottery());
        lotteryGold.LotteryStart();
    }

    IEnumerator Lottery()
    {
        soundManager.PlayBgm((int)StageSoundManager.StageBgm.lotteryStart);
        //애니메이션 시간동안 대기
        lotteryHighlightAnimation.SetActive(true);
        yield return new WaitForSeconds(2.5f);

        lotteryGold.LotteryStop();
        lotteryHighlightAnimation.SetActive(false);
        int quo = lotteryTargetIdx / 4, rem = lotteryTargetIdx % 4;
        Vector2 posVec;
        switch(quo)
        {
            case 0:
                posVec = new Vector2(-180 + (90 * rem) , 180);
                break;
            case 1:
                posVec = new Vector2(180, 180 - (90 * rem));
                break;
            case 2:
                posVec = new Vector2(180 - (90 * rem), -180);
                break;
            case 3:
                posVec = new Vector2(-180, -180 + (90 * rem));
                break;
            default:
                posVec = new Vector3(-180, 180);
                break;
        }
        lotteryHighlight.rectTransform.anchoredPosition = posVec;
        lotteryHighlight.gameObject.SetActive(true);

        soundManager.StopBgm();
        soundManager.PlaySfx((int)StageSoundManager.StageSfx.lotteryEnd);
        yield return new WaitForSeconds(0.5f);

        lotteryResultIcon.sprite = SpriteContainer.getSprite(lotteryTargetData.id);
        lotteryResName.text = lotteryTargetData.name;
        lotteryResDesc.text = lotteryTargetData.description; 
        //업그레이드 가능한 무기 존재 시
        if (lotteryTargetData.id % 10 == 9)
        {
            lotteryStarSet.SetActive(false);
            lotteryResStarForLegandary.SetActive(true);
        }
        else
        {
            lotteryStarSet.SetActive(true);
            lotteryResStarForLegandary.SetActive(false);

            for (int i = 0; i < lotteryTargetData.level; i++)
                lotteryResStars[i].SetActive(true);
            for (int i = lotteryTargetData.level; i < lotteryResStars.Length; i++)
                lotteryResStars[i].SetActive(false);
        }
        lotteryResultSet.SetActive(true);
    }

    public void Btn_LotteryEnd()
    {
        weaponLogic.GetWeapon(lotteryTargetData.id);
        lotteryResultSet.SetActive(false);
        lotteryHighlight.gameObject.SetActive(false);
        lotterySet.SetActive(false);
        lotteryStartBtn.SetActive(true);

        //골드 갱신
        GoldManager.Instance.PlusGold(lotteryGold.Gold);
        gameManager.EndTreasureBox(lotteryGold.Gold);

        //일시정지 해제 및 무기 코루틴 재시작
        gameManager.PauseOff();
        weaponLogic.RestartWeapons();
        soundManager.PlayBgm((int)StageSoundManager.StageBgm.stage_1);
    }

    public void InputEnter()
    {
        if (!weaponSelectSet.activeSelf && !lotterySet.activeSelf)
            return;

        //무기 선택 창에서 Enter 입력 시
        if(weaponSelectSet.activeSelf)
        {
            Btn_WeaponSelectComplete();
            return;
        }

        //럭키 찬스 창에서 Enter 입력 시
        if(lotteryStartBtn.activeSelf)
            Btn_StartLottery();
    }

    //스테이지 클리어
    public void StageClear(int kill, int money)
    {
        soundManager.PlaySfx((int)StageSoundManager.StageSfx.stageClear);
        weaponLogic.AllStop();
        killCountText_stageClear.text = string.Format("{0:n0}", kill);
        moneyCountText_stageClear.text = string.Format("{0:n0}", money);
        stageClearSet.SetActive(true);
    }

    public void Btn_StageClear()
    {
        stageClearSet.SetActive(false);
        LoadingSceneManager.exitStage();
    }

    //게임 오버
    public void GameOver(int kill, int money)
    {
        soundManager.PlaySfx((int)StageSoundManager.StageSfx.gameOver);
        weaponLogic.AllStop();
        killCountText_gameover.text = string.Format("{0:n0}", kill);
        moneyCountText_gameover.text = string.Format("{0:n0}", money);
        gameOverSet.SetActive(true);
    }

    public void Btn_GameOver()
    {
        gameOverSet.SetActive(false);
        LoadingSceneManager.exitStage();
    }
}