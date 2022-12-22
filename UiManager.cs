using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    //전투 관련
    [Header("전투")]
    [SerializeField] Image hpBarSet;
    [SerializeField] Image hpBar;
    Vector3 hpPos;
    Vector3 hpPosOffset;
    Vector3 hpBarScale;

    //경험치 관련
    [Header("경험치")]
    [SerializeField] Image expBar;
    [SerializeField] TextMeshProUGUI levelText;
    Vector3 expBarScale;

    //무기 획득 관련
    [Header("무기 획득")]
    [SerializeField] GameObject weaponSelectSet;
    [SerializeField] GameObject[] weaponSelectSlots;
    WeaponData[] weaponDatas;
    int curWeaponIdx;
    [SerializeField] Image[] weaponImages;
    [SerializeField] TextMeshProUGUI[] weaponName;
    [SerializeField] TextMeshProUGUI[] weaponDesc;

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

    public static Action<int, Vector3> showDamage;

    public void GetWeaponLogic(Weapons weaponLogic)
    {
        this.weaponLogic = weaponLogic;
    }

    void Awake()
    {
        showDamage = (a, b) => { StartCoroutine(ShowDamage(a, b)); };

        hpPosOffset = new Vector3(0, -60, 0);
        hpBarScale = Vector3.one;

        expBarScale = Vector3.one;
    }

    void Start()
    {
        UpdateKillCount(0);
        UpdateMoneyCount(0);
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

    public void UpdateMoneyCount(int count)
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
        weaponDatas = weaponLogic.GetRandomWeaponData().ToArray();
        for (int i = 0; i < weaponDatas.Length; i++)
        {
            weaponImages[i].sprite = SpriteContainer.getSprite(weaponDatas[i].WeaponId);
            weaponName[i].text = weaponDatas[i].WeaponName;
            weaponDesc[i].text = weaponDatas[i].WeaponDescription;
            weaponSelectSlots[i].SetActive(true);
        }
        //획득 또는 업그레이드 가능한 무기가 3개 미만일 경우
        for (int i = weaponDatas.Length; i < 3; i++)
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
        weaponLogic.GetWeapon(weaponDatas[curWeaponIdx].WeaponId);
        weaponSelectSet.SetActive(false);
        gameManager.PauseOff();

        weaponLogic.RestartWeapons();
        gameManager.LevelUp();
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
