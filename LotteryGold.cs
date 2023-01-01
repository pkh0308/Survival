using UnityEngine;
using TMPro;

//럭키 찬스 골드 노출용 클래스
//활성화 시 자동으로 골드 증가 및 갱신, UiManager에서 Stop 호출하여 정지
public class LotteryGold : MonoBehaviour
{
    TextMeshProUGUI goldText;
    int gold;
    public int Gold { get { return gold; } }
    [SerializeField] int minGold;
    [SerializeField] int maxGold;
    [SerializeField] int devider;
    bool isLottering;

    void Awake()
    {
        goldText = GetComponentInChildren<TextMeshProUGUI>();
    }

    void OnEnable()
    {
        gold = 0;
    }

    public void LotteryStart()
    {
        isLottering = true;
    }

    void Update()
    {
        if (!isLottering) return;

        gold += Random.Range(minGold, maxGold) / devider;
        goldText.text = string.Format("{0:n0}", gold);
    }

    public void LotteryStop()
    {
        isLottering = false;
    }
}