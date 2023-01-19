using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

//씬 관리 및 로딩스크린 노출용 클래스
public class LoadingSceneManager : MonoBehaviour
{
    [SerializeField] GameObject loadingScene;
    [SerializeField] GameObject loadingCamera;

    public static LoadingSceneManager Inst { get; private set; }

    int curStageIdx;
    public int CurStageIdx { get { return curStageIdx; } }
    WaitForSeconds minInterval = new WaitForSeconds(0.1f);

    public enum SceneIndex
    {
        LOADING = 0,
        LOBBY = 1,
        PLAYER = 2,
        STAGE_1 = 3,
        STAGE_2,
        STAGE_3,
        STAGE_4
    }

    void Awake()
    {
        Inst = this;
        Application.targetFrameRate = 60;

        loadingScene.SetActive(true);
        LoadLobby();
    }

    public void LoadLobby()
    {
        if(SceneManager.GetActiveScene() != SceneManager.GetSceneByBuildIndex((int)SceneIndex.LOBBY))
            StartCoroutine(LoadingLobby());
    }

    public void SetActiveSceneToCurStage()
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(curStageIdx));
    }

    public void SetActiveSceneToPlayerScene()
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex((int)SceneIndex.PLAYER));
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

    IEnumerator LoadingLobby()
    {
        //로비 씬 로드 대기
        AsyncOperation op = SceneManager.LoadSceneAsync((int)SceneIndex.LOBBY, LoadSceneMode.Additive);
        while (!op.isDone)
        {
            yield return minInterval;
        }
        loadingScene.SetActive(false);
        loadingCamera.SetActive(false);
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex((int)SceneIndex.LOBBY));
    }

    //입장하는 스테이지를 LoadSceneAsync로 로딩하며 AsyncOperation 변수에 저장
    //로딩 작업이 완료될때까지 대기한 후 해당 스테이지를 액티브 씬으로 설정, 이후 오브젝트 생성
    IEnumerator Loading()
    {
        //스테이지 씬 로드 대기
        AsyncOperation op = SceneManager.LoadSceneAsync(curStageIdx, LoadSceneMode.Additive);
        while (!op.isDone)
        {
            yield return minInterval;
        }
        //플레이어 씬 로드 대기
        op = SceneManager.LoadSceneAsync((int)SceneIndex.PLAYER, LoadSceneMode.Additive);
        while (!op.isDone)
        {
            yield return minInterval;
        }
        loadingScene.SetActive(false);
    }
}