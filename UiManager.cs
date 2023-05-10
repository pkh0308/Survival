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
    Vector3 expBarScale = Vector3.one;

    //무기 획득 관련
    [Header("무기 획득")]
    [SerializeField] GameObject weaponSelectSet;
    [SerializeField] GameObject[] weaponSelectSlots;
    Button[] weaponSelectSlots_Btn;
    Image[] weaponSelectSlots_Bg;

    DataForLevelUp[] levelupDatas;
    int curWeaponIdx;
    [SerializeField] Image[] weaponImages;
    [SerializeField] TextMeshProUGUI[] weaponName;
    [SerializeField] TextMeshProUGUI[] weaponDesc;
    [SerializeField] Image[] levelUpStars_0;
    [SerializeField] Image[] levelUpStars_1;
    [SerializeField] Image[] levelUpStars_2;
    List<Image[]> levelUpStars;

    //보물상자 획득 관련
    [Header("보물 상자")]
    [SerializeField] GameObject lotterySet;
    [SerializeField] GameObject lotteryStartBtn;
    [SerializeField] RectTransform[] lotterySlots;
    [SerializeField] Image[] lotterySlotIcons;
    [SerializeField] GameObject lotteryHighlightAnimation;
    [SerializeField] Image lotteryHighlight;
    [SerializeField] TextMeshProUGUI lotteryGoldText;
    [SerializeField] GameObject lotteryResultSet;
    [SerializeField] Image lotteryResultIcon;
    [SerializeField] TextMeshProUGUI lotteryResName;
    [SerializeField] TextMeshProUGUI lotteryResDesc;
    [SerializeField] Image[] lotteryResStars;
    [SerializeField] Image lotteryResSlot;
    [SerializeField] LotteryGold lotteryGold;
    DataForLevelUp lotteryTargetData;
    int lotteryTargetIdx;
    Vector2 highlightOffset = new Vector2(40, -40);

    [Header("보스")]
    [SerializeField] GameObject bossAlertSet;
    [SerializeField] GameObject bossIcon;
    [SerializeField] Image bossHpBar;
    Vector3 bossHpBarScale = Vector3.one;
    bool onBossFight;
    public static Action<int, int> updateBossHp;

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

        updateBossHp = (cur, max) => { UpdateBossHp(cur, max); };

        weaponSelectSlots_Bg = new Image[weaponSelectSlots.Length];
        for (int i = 0; i < weaponSelectSlots.Length; i++)
            weaponSelectSlots_Bg[i] = weaponSelectSlots[i].GetComponent<Image>();

        weaponSelectSlots_Btn = new Button[weaponSelectSlots.Length];
        for (int i = 0; i < weaponSelectSlots.Length; i++)
            weaponSelectSlots_Btn[i] = weaponSelectSlots[i].GetComponent<Button>();

        levelUpStars = new List<Image[]>();
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

        yield return WfsManager.Instance.GetWaitForSeconds(1.0f);
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
        //일시정지 화면일 경우 나가며 false 반환
        //일시정지 상태가 아닐 경우 일시정지 화면 오픈하며 true 반환
        pauseSet.SetActive(!pause);
        return pauseSet.activeSelf;
    }

    public void Btn_ExitStage()
    {
        exitPopupSet.SetActive(!exitPopupSet.activeSelf);
    }

    public void Btn_ExitYes()
    {
        LoadingSceneManager.Inst.ExitStage();
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

    //보스 관련
    public IEnumerator BossAlert(float interval)
    {
        bossAlertSet.SetActive(true);
        soundManager.PlaySfx((int)StageSoundManager.StageSfx.bossAlert);
        yield return WfsManager.Instance.GetWaitForSeconds(interval);

        bossAlertSet.SetActive(false);
    }

    public void ChangeProgressBar()
    {
        if(!onBossFight)
        {
            onBossFight = true;
            expBar.gameObject.SetActive(false);
            bossHpBar.gameObject.SetActive(true);
            levelText.gameObject.SetActive(false);
            bossIcon.SetActive(true);
        }
        else
        {
            onBossFight = false;
            expBar.gameObject.SetActive(true);
            bossHpBar.gameObject.SetActive(false);
            levelText.gameObject.SetActive(true);
            bossIcon.SetActive(false);
        }
    }

    public void UpdateBossHp(int cur, int max)
    {
        bossHpBarScale.x = (float)cur / max;
        bossHpBar.rectTransform.localScale = bossHpBarScale;
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
        levelupDatas = new DataForLevelUp[(int)Weapons.WeaponVar.NumOfLevelUpWeapon];
        weaponLogic.GetRandomWeaponData(levelupDatas);
        for (int i = 0; i < levelupDatas.Length; i++)
        {
            weaponImages[i].sprite = SpriteContainer.getSprite(levelupDatas[i].id);
            weaponName[i].text = levelupDatas[i].name;
            weaponDesc[i].text = levelupDatas[i].description;
            //업그레이드 무기일 경우
            if(levelupDatas[i].id % 10 == (int)Weapons.WeaponVar.UpgradeWeapon)
            {
                int middle = (int)Weapons.WeaponVar.MaxLevelOfWeapon / 2;
                for (int j = 0; j < (int)Weapons.WeaponVar.MaxLevelOfWeapon; j++)
                {
                    if (j == middle) // 가운데 별만 활성화
                    {
                        levelUpStars[i][j].sprite = SpriteContainer.getSprite(ObjectNames.legandaryStar);
                        levelUpStars[i][j].gameObject.SetActive(true);
                        continue;
                    }
                    levelUpStars[i][j].gameObject.SetActive(false); // 가운데 외 비활성화
                }
                weaponSelectSlots_Bg[i].sprite = SpriteContainer.getSprite(ObjectNames.weaponSlotLegandary);
            }
            else
            {
                if (levelupDatas[i].maxLevel == 1)
                {
                    for (int j = 0; j < (int)Weapons.WeaponVar.MaxLevelOfWeapon; j++)
                    {
                        if (j == 2) // 가운데 별만 활성화
                        {
                            levelUpStars[i][j].sprite = SpriteContainer.getSprite(ObjectNames.normalStar);
                            levelUpStars[i][j].gameObject.SetActive(true);
                            continue;
                        }
                        levelUpStars[i][j].gameObject.SetActive(false); // 가운데 외 비활성화
                    }
                }
                else if (levelupDatas[i].maxLevel == 3)
                {
                    levelUpStars[i][0].gameObject.SetActive(false);
                    levelUpStars[i][4].gameObject.SetActive(false);

                    for (int j = 1; j < levelUpStars[i].Length - 1; j++)
                    {
                        if (j <= levelupDatas[i].level)
                            levelUpStars[i][j].sprite = SpriteContainer.getSprite(ObjectNames.normalStar);
                        else
                            levelUpStars[i][j].sprite = SpriteContainer.getSprite(ObjectNames.blackStar);

                        levelUpStars[i][j].gameObject.SetActive(true);
                    }
                }
                else
                {
                    for (int j = 0; j < levelUpStars[i].Length; j++)
                    {
                        if (j < levelupDatas[i].level)
                            levelUpStars[i][j].sprite = SpriteContainer.getSprite(ObjectNames.normalStar);
                        else
                            levelUpStars[i][j].sprite = SpriteContainer.getSprite(ObjectNames.blackStar);

                        levelUpStars[i][j].gameObject.SetActive(true);
                    }
                }
                weaponSelectSlots_Bg[i].sprite = SpriteContainer.getSprite(ObjectNames.weaponSlotNormal);
            }

            weaponSelectSlots[i].SetActive(true);
        }
        //획득 또는 업그레이드 가능한 무기가 3개 미만일 경우
        for (int i = levelupDatas.Length; i < (int)Weapons.WeaponVar.NumOfLevelUpWeapon; i++)
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
        gameManager.PauseOff();
        weaponLogic.GetWeapon(levelupDatas[curWeaponIdx].id);
        weaponSelectSet.SetActive(false);
        //선택된 버튼 초기화
        weaponSelectSlots_Btn[curWeaponIdx].interactable = false;
        weaponSelectSlots_Btn[curWeaponIdx].interactable = true;
        curWeaponIdx = -1;
    }

    public void InputUpDown(KeyCode flag)
    {
        //무기 선택창 활성화 상태에서만 동작
        if(!weaponSelectSet.activeSelf) return;

        if(flag == KeyCode.UpArrow)
        {
            curWeaponIdx--;
            if (curWeaponIdx < 0)
                curWeaponIdx = weaponSelectSlots.Length - 1;
        }
        else
        {
            curWeaponIdx++;
            if (curWeaponIdx >= weaponSelectSlots.Length)
                curWeaponIdx = 0;
        }
        weaponSelectSlots_Btn[curWeaponIdx].Select();
    }

    //무기, 악세사리 목록 업데이트
    public void UpdateWeaponAccList(int id)
    {
        if(id < (int)Weapons.WeaponVar.WeaponBoundary) //무기
        {
            if(id % 10 == (int)Weapons.WeaponVar.UpgradeWeapon) //업그레이드 무기일 경우
            {
                int beforeId = id / 10 * 10 + 1; // 업그레이드 무기의 기본형 ID
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
        lotteryTargetIdx = Random.Range(0, lotterySlotIcons.Length);
        for (int i = 0; i < lotterySlotIcons.Length; i++)
        {
            idx = Random.Range(0, datas.Length);
            lotterySlotIcons[i].sprite = SpriteContainer.getSprite(datas[idx].id);

            if (i == lotteryTargetIdx)
                lotteryTargetData = datas[idx];
        }

        //업그레이드 가능한 무기가 있을 경우 무작위 슬롯 하나를 덮어씌우고,
        //lotteryTargetData를 업그레이드 무기로 고정
        if (id > 0) 
        {
            idx = Random.Range(0, lotterySlotIcons.Length);
            lotterySlotIcons[idx].sprite = SpriteContainer.getSprite(id);
            lotteryTargetIdx = idx;
            lotteryTargetData = datas[0];
        }

        lotteryStartBtn.SetActive(true);
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
        yield return WfsManager.Instance.GetWaitForSeconds(2.5f);

        lotteryGold.LotteryStop();
        lotteryHighlightAnimation.SetActive(false);
        lotteryHighlight.rectTransform.anchoredPosition = lotterySlots[lotteryTargetIdx].anchoredPosition + highlightOffset;
        lotteryHighlight.gameObject.SetActive(true);

        soundManager.StopBgm();
        soundManager.PlaySfx((int)StageSoundManager.StageSfx.lotteryEnd);
        yield return WfsManager.Instance.GetWaitForSeconds(0.5f);

        lotteryResultIcon.sprite = SpriteContainer.getSprite(lotteryTargetData.id);
        lotteryResName.text = lotteryTargetData.name;
        lotteryResDesc.text = lotteryTargetData.description; 
        //업그레이드 무기
        if (lotteryTargetData.id % 10 == 9)
        {
            for(int i = 0; i < 5; i++)
            {
                if(i == 2)
                {
                    lotteryResStars[i].sprite = SpriteContainer.getSprite(ObjectNames.legandaryStar);
                    lotteryResStars[i].gameObject.SetActive(true);
                    continue;
                }
                lotteryResStars[i].gameObject.SetActive(false);
            }
            lotteryResSlot.sprite = SpriteContainer.getSprite(ObjectNames.weaponSlotLegandary);
        }
        else //일반 무기 or 악세사리
        {
            //보물상자는 획득한 장비만 나오므로 최대 1레벨은 생략
            if(lotteryTargetData.maxLevel == 3)
            {
                lotteryResStars[0].gameObject.SetActive(false);
                lotteryResStars[4].gameObject.SetActive(false);

                for(int i = 1; i < lotteryResStars.Length - 1; i++)
                {
                    if (i <= lotteryTargetData.level) 
                        lotteryResStars[i].sprite = SpriteContainer.getSprite(ObjectNames.normalStar);
                    else
                        lotteryResStars[i].sprite = SpriteContainer.getSprite(ObjectNames.blackStar);

                    lotteryResStars[i].gameObject.SetActive(true);
                }
            }
            else
            {
                for (int i = 0; i < lotteryResStars.Length; i++)
                {
                    if (i < lotteryTargetData.level)
                        lotteryResStars[i].sprite = SpriteContainer.getSprite(ObjectNames.normalStar);
                    else
                        lotteryResStars[i].sprite = SpriteContainer.getSprite(ObjectNames.blackStar);

                    lotteryResStars[i].gameObject.SetActive(true);
                }
            }

            lotteryResSlot.sprite = SpriteContainer.getSprite(ObjectNames.weaponSlotNormal);
        }
        lotteryResultSet.SetActive(true);
    }

    public void Btn_LotteryEnd()
    {
        weaponLogic.GetWeapon(lotteryTargetData.id);
        lotteryResultSet.SetActive(false);
        lotteryHighlight.gameObject.SetActive(false);
        lotterySet.SetActive(false);

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
        //무기 선택 창에서 Enter 입력 시
        if(weaponSelectSet.activeSelf)
        {
            Btn_WeaponSelectComplete();
            return;
        }

        //럭키 찬스 창에서 Enter 입력 시
        if (lotteryStartBtn.activeSelf)
        {
            Btn_StartLottery();
            return;
        }
        if (lotteryResultSet.activeSelf)
        {
            Btn_LotteryEnd();
            return;
        }
            
        //스테이지 클리어 or 게임 오버
        if (gameOverSet.activeSelf)
        {
            Btn_GameOver();
            return;
        }
        if (stageClearSet.activeSelf)
        {
            Btn_StageClear();
            return;
        }
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
        LoadingSceneManager.Inst.ExitStage();
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
        LoadingSceneManager.Inst.ExitStage();
    }
}