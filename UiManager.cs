﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UiManager : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] Weapons weaponLogic;

    //타이머 관련
    [SerializeField] TextMeshProUGUI timerText;

    //카운트 관련
    [SerializeField] TextMeshProUGUI killCountText;
    [SerializeField] TextMeshProUGUI goldCountText;

    //일시정지 관련
    [SerializeField] GameObject pauseSet;
    [SerializeField] GameObject exitPopupSet;
    int mm = 0;
    int ss = 0;

    //전투 관련
    [SerializeField] Image hpBarSet;
    [SerializeField] Image hpBar;
    Vector3 hpPos;
    Vector3 hpPosOffset;
    Vector3 hpBarScale;

    //경험치 관련
    [SerializeField] Image expBar;
    [SerializeField] TextMeshProUGUI levelText;
    Vector3 expBarScale;

    //무기 획득 관련
    [SerializeField] GameObject weaponSelectSet;


    void Awake()
    {
        hpPosOffset = new Vector3(0, -60, 0);
        hpBarScale = Vector3.one;

        expBarScale = Vector3.one;
    }

    void Start()
    {
        UpdateKillCount(0);
        UpdateGoldCount(0);
    }

    void Update()
    {
        
    }

    void LateUpdate()
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
        expBarScale.x = (float)cur / max;
        expBar.rectTransform.localScale = expBarScale;
    }

    public void UpdateLevel(int level)
    {
        levelText.text = "Lv." + level;
    }

    public void WeaponSelect()
    {
        weaponSelectSet.SetActive(true);
    }

    public void Btn_WeaponSelectComplete(int id)
    {
        weaponLogic.GetWeapon(id);
        gameManager.PauseOff();
    }
}