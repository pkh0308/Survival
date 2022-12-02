using UnityEngine;

//강화 버튼에 데이터 저장용 클래스
public class EnhanceSlot : MonoBehaviour
{
    int enhanceId;
    public int EnhanceId { get { return enhanceId; } }

    public void SetEnhanceId(int id)
    {
        enhanceId = id;
    }
}
