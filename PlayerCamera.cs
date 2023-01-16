using UnityEngine;
using System.IO;

//카메라 위치 갱신용 클래스
//플레이어를 따라다니도록 설정
public class PlayerCamera : MonoBehaviour
{
    float hMax, hMin, vMax, vMin;

    Vector3 cameraPos;

    void Awake()
    {
        cameraPos = new Vector3(0, 0, -10);

        ReadData();
    }

    void ReadData()
    {
        //csv 파일에서 데이터 읽어오기
        TextAsset stageDatas = Resources.Load("StageDatas") as TextAsset;
        StringReader stageDataReader = new StringReader(stageDatas.text);

        if (stageDataReader == null)
        {
            Debug.Log("stageDataReader is null");
            return;
        }
        //첫줄 스킵(변수 이름 라인)
        string line = stageDataReader.ReadLine();
        if (line == null) return;

        line = stageDataReader.ReadLine();
        while (line.Length > 1)
        {
            string[] datas = line.Split(',');
            if(datas[0] != LoadingSceneManager.Inst.CurStageIdx.ToString())
            {
                line = stageDataReader.ReadLine();
                continue;
            }

            // 1: hMin, 2: hMax, 3: vMin, 4: vMax
            hMin = float.Parse(datas[1]);
            hMax = float.Parse(datas[2]);
            vMin = float.Parse(datas[3]);
            vMax = float.Parse(datas[4]);
            break;
        }
        stageDataReader.Close();
    }

    void LateUpdate()
    {
        cameraPos.x = Mathf.Clamp(Player.playerPos.x, hMin, hMax);
        cameraPos.y = Mathf.Clamp(Player.playerPos.y, vMin, vMax);
        transform.position = cameraPos;
    }
}