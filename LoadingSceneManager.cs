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
    }

    public void LoadLobby()
    {
        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByBuildIndex((int)SceneIndex.LOBBY))
            LoadAsync_Lobby();
    }

    //스테이지 입장 시
    public void EnterStage(int stageIdx)
    {
        curStageIdx = stageIdx + 2;
        loadingScene.SetActive(true);
        SceneManager.UnloadSceneAsync((int)SceneIndex.LOBBY);
        LoadAsync_Player();
    }

    public void ExitStage()
    {
        loadingScene.SetActive(true);
        SceneManager.UnloadSceneAsync(curStageIdx);
        SceneManager.UnloadSceneAsync((int)SceneIndex.PLAYER);
        LoadAsync_Lobby();
    }

    // 로비 씬 로드(비동기)
    void LoadAsync_Lobby()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync((int)SceneIndex.LOBBY, LoadSceneMode.Additive);
        op.completed += (o) =>
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex((int)SceneIndex.LOBBY));
            OnLoadComplete();
            SoundManager.playBgm(SoundManager.StageBgm.lobbyBgm);
        };
    }
    // 플레이어 씬 로딩(비동기)
    // 완료 시 LoadAsync_Stage() 호출
    void LoadAsync_Player()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync((int)SceneIndex.PLAYER, LoadSceneMode.Additive);
        op.completed += (o) =>
        {
            LoadAsync_Stage();
        };
    }
    // 스테이지 로딩(비동기)
    void LoadAsync_Stage()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(curStageIdx, LoadSceneMode.Additive);
        op.completed += (o) =>
        {
            // 플레이어 초기화
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex((int)SceneIndex.PLAYER));
            ObjectManager.startCharacter();
            // 스테이지 초기화
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(curStageIdx));
            ObjectManager.generate();

            OnLoadComplete();
            SoundManager.playBgm(SoundManager.StageBgm.stage_1);
        };
    }
    
    void OnLoadComplete()
    {
        loadingScene.SetActive(false);
        loadingCamera.SetActive(false);
    }
}