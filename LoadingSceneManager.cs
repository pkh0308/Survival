using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingSceneManager : MonoBehaviour
{
    [SerializeField] GameObject loadingScene;

    public static Action<int> enterStage;
    public static Action setActiveScene;
    public static Action exitStage;

    int curStageIdx;

    public enum SceneIndex
    {
        LOADING = 0,
        LOBBY = 1,
        PLAYER = 2,
        STAGE_1 = 3
    }

    void Awake()
    {
        enterStage = (a) => { EnterStage(a); };
        setActiveScene = () => { SetActiveScene(); };
        exitStage = () => { ExitStage(); };

        LoadLobby();
    }

    public void LoadLobby()
    {
        SceneManager.LoadScene((int)SceneIndex.LOBBY, LoadSceneMode.Additive);
    }

    public void SetActiveScene()
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(curStageIdx));
    }

    //스테이지 입장 시
    public void EnterStage(int stageIdx)
    {
        curStageIdx = stageIdx + 2;
        loadingScene.SetActive(true);
        SceneManager.UnloadSceneAsync((int)SceneIndex.LOBBY);
        StartCoroutine(Loading());
    }

    public void ExitStage()
    {
        loadingScene.SetActive(true);
        SceneManager.UnloadSceneAsync(curStageIdx);
        SceneManager.UnloadSceneAsync((int)SceneIndex.PLAYER);
        SceneManager.LoadScene((int)SceneIndex.LOBBY, LoadSceneMode.Additive);
        loadingScene.SetActive(false);
    }

    //입장하는 스테이지를 LoadSceneAsync로 로딩하며 AsyncOperation 변수에 저장
    //로딩 작업이 완료될때까지 대기한 후 해당 스테이지를 액티브 씬으로 설정, 이후 오브젝트 생성
    IEnumerator Loading()
    {
        WaitForSeconds seconds = new WaitForSeconds(0.1f);
        //스테이지 씬 로드 대기
        AsyncOperation op = SceneManager.LoadSceneAsync(curStageIdx, LoadSceneMode.Additive);
        while (!op.isDone)
        {
            yield return seconds;
        }
        //플레이어 씬 로드 대기
        op = SceneManager.LoadSceneAsync((int)SceneIndex.PLAYER, LoadSceneMode.Additive);
        while (!op.isDone)
        {
            yield return seconds;
        }

        yield return new WaitForSeconds(0.5f); //로딩 화면 체크용 추가 시간, 나중에 삭제할 것
        loadingScene.SetActive(false);
    }
}
