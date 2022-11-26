using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] ImageContainer imgContainer;

    [SerializeField] GameObject characterSelectSet;
    [SerializeField] GameObject stageSelectSet;
    [SerializeField] GameObject enhancementSet;
    [SerializeField] GameObject exitSet;

    //캐릭터 선택 화면 관련
    int charId;

    //스테이지 선택 화면 관련
    int stageId;

    //강화 화면 관련
    Dictionary<int, EnhancementData> enhanceDic;
    public int initialEnhanceId;

    int curEnhanceId;
    [SerializeField] Image enhanceIcon;
    [SerializeField] TextMeshProUGUI enhanceNameText;
    [SerializeField] TextMeshProUGUI enhanceDescText;

    void Awake()
    {
        charId = 0;
        stageId = 0;

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

                //0: id, 1: spriteId, 2: name, 3: desc, 4: type, 5: val
                EnhancementData e_data = new EnhancementData(id, int.Parse(datas[1]), datas[2], datas[3], int.Parse(datas[4]), float.Parse(datas[5]));
                enhanceDic.Add(id, e_data);

                line = enhanceDataReader.ReadLine();
                if (line == null) break;
            }
        }
        enhanceDataReader.Close();
    }

    //게임 시작 버튼 관련
    public void Btn_GameStart()
    {
        characterSelectSet.SetActive(true);
    }

    public void Btn_characterSelect()
    {
        if (charId == 0) return;

        characterSelectSet.SetActive(false);
        stageSelectSet.SetActive(true);
    }

    public void Btn_Character(int id)
    {
        charId = id;
    }

    public void Btn_characterSelectExit()
    {
        characterSelectSet.SetActive(false);
        charId = 0;
    }

    public void Btn_StageSelectExit()
    {
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

        LoadingSceneManager.enterStage(stageId);
    }

    //강화 버튼 관련
    public void Btn_Enhancement()
    {
        if(enhancementSet.activeSelf)
        {
            enhancementSet.SetActive(false);

            enhanceIcon.sprite = imgContainer.GetSprite(enhanceDic[initialEnhanceId].spriteId);
            enhanceNameText.text = enhanceDic[initialEnhanceId].nameText;
            enhanceDescText.text = enhanceDic[initialEnhanceId].descText;
        }
        else
            enhancementSet.SetActive(true);
    }

    public void Btn_EnhanceSelect(int id)
    {
        curEnhanceId = id;

        enhanceIcon.sprite = imgContainer.GetSprite(enhanceDic[curEnhanceId].spriteId);
        enhanceNameText.text = enhanceDic[curEnhanceId].nameText;
        enhanceDescText.text = enhanceDic[curEnhanceId].descText;
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
