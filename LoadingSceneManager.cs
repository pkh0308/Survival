using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingSceneManager : MonoBehaviour
{
    [SerializeField] GameObject loadingScene;

    public static Action<int> enterStage;
    public static Action<int> setActiveScene;
    public static Action exitStage;

    int curStageIdx;

    public enum SceneIndex
    {
        LOADING = 0,
        LOBBY = 1,
        STAGE_1 = 2
    }

    void Awake()
    {
        enterStage = (a) => { EnterStage(a); };
        setActiveScene = (a) => { SetActiveScene(a); };
        exitStage = () => { ExitStage(); };

        LoadLobby();
    }

    public void LoadLobby()
    {
        SceneManager.LoadScene((int)SceneIndex.LOBBY, LoadSceneMode.Additive);
    }

    public void SetActiveScene(int sceneIdx)
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(sceneIdx));
    }

    //스테이지 입장 시
    public void EnterStage(int stageIdx)
    {
        curStageIdx = stageIdx + 1;
        loadingScene.SetActive(true);
        SceneManager.UnloadSceneAsync((int)SceneIndex.LOBBY);
        StartCoroutine(Loading(stageIdx));
    }

    public void ExitStage()
    {
        loadingScene.SetActive(true);
        SceneManager.UnloadSceneAsync(curStageIdx);
        SceneManager.LoadScene((int)SceneIndex.LOBBY, LoadSceneMode.Additive);
        loadingScene.SetActive(false);
    }

    //입장하는 스테이지를 LoadSceneAsync로 로딩하며 AsyncOperation 변수에 저장
    //로딩 작업이 완료될때까지 대기한 후 해당 스테이지를 액티브 씬으로 설정, 이후 오브젝트 생성
    IEnumerator Loading(int idx)
    {
        WaitForSeconds seconds = new WaitForSeconds(0.1f);
        AsyncOperation op = SceneManager.LoadSceneAsync(idx + 1, LoadSceneMode.Additive);
        //로딩 완료될때까지 대기
        while (!op.isDone)
        {
            yield return seconds;
        }
        yield return new WaitForSeconds(0.5f); //로딩 화면 체크용 추가 시간, 나중에 삭제할 것
        loadingScene.SetActive(false);
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(idx + 1));
    }
}
