//악세사리 정보를 csv 파일에서 읽어들인 뒤 저장하기 위한 클래스
public class AccessoryData
{
    //아이디
    int accesoryId;
    public int AccesoryId { get { return accesoryId; } }
    //레벨
    int accessoryLevel;
    public int AccessoryLevel { get { return accessoryLevel; } }
    //이름
    string accessoryName;
    public string AccessoryName { get { return accessoryName; } }
    //값
    float accessoryValue;
    public float AccessoryValue { get { return accessoryValue; } }
    //설명
    string accessoryDescription;
    public string AccessoryDescription { get { return accessoryDescription; } }

    public AccessoryData(int id, int lv, string name, float val, string desc)
    {
        accesoryId = id;
        accessoryLevel = lv;
        accessoryName = name;
        accessoryValue = val;
        accessoryDescription = desc;
    }
}