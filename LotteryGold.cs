using UnityEngine;
using TMPro;

//��Ű ���� ��� ����� Ŭ����
//Ȱ��ȭ �� �ڵ����� ��� ���� �� ����, UiManager���� Stop ȣ���Ͽ� ����
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