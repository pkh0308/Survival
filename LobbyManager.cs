using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] GameObject characterSelectSet;
    [SerializeField] GameObject stageSelectSet;
    [SerializeField] GameObject enhancementSet;
    [SerializeField] GameObject exitSet;

    int curStageIdx;

    void Awake()
    {
        curStageIdx = 1;
    }

    //게임 시작 버튼 관련
    public void Btn_GameStart()
    {
        characterSelectSet.SetActive(true);
    }

    public void Btn_characterSelect()
    {
        characterSelectSet.SetActive(false);
        stageSelectSet.SetActive(true);
    }

    public void Btn_characterSelectExit()
    {
        characterSelectSet.SetActive(false);
    }

    public void Btn_StageSelectExit()
    {
        stageSelectSet.SetActive(false);
    }

    public void Btn_StageEnter()
    {
        LoadingSceneManager.enterStage(curStageIdx);
    }

    //강화 버튼 관련
    public void Btn_Enhancement()
    {
        if(enhancementSet.activeSelf)
            enhancementSet.SetActive(false);
        else
            enhancementSet.SetActive(true);
    }

    public void Btn_Enhance()
    {

    }

    //종료 버튼 관련
    public void Btn_Exit()
    {
        if(exitSet.activeSelf)
            exitSet.SetActive(false);
        else
            exitSet.SetActive(true);
    }

    public void Btn_ExitYes()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
}
