using UnityEngine;

public class Prefers
{
    //싱글톤 구현
    private static Prefers instance;
    public static Prefers Instance
    {
        get
        {
            if (instance == null)
                instance = new Prefers();

            return instance;
        }
    }

    int atkPower;
    public int AtkPower { get { return atkPower; } }
    int atkScale;
    public int AtkScale { get { return atkScale; } }
    int projSpeed;
    public int ProjSpeed { get { return projSpeed; } }
    int coolTime;
    public int CoolTime { get { return coolTime; } }
    int projCount;
    public int ProjCount { get { return projCount; } }
    int atkRemainTime;
    public int AtkRemainTime { get { return atkRemainTime; } }
    int playerHealth;
    public int PlayerHealth { get { return playerHealth; } }
    int playerdef;
    public int Playerdef { get { return playerdef; } }
    int playerMoveSpeed;
    public int PlayerMoveSpeed { get { return playerMoveSpeed; } }

    //생성 시 PlayerPrefs 값이 있으면 해당 값을, 없을 경우 초기값 저장
    public Prefers()
    {
        atkPower = PlayerPrefs.HasKey(nameof(atkPower)) ? PlayerPrefs.GetInt(nameof(atkPower)) : 100;
        atkScale = PlayerPrefs.HasKey(nameof(atkScale)) ? PlayerPrefs.GetInt(nameof(atkScale)) : 200;
        projSpeed = PlayerPrefs.HasKey(nameof(projSpeed)) ? PlayerPrefs.GetInt(nameof(projSpeed)) : 300;
        coolTime = PlayerPrefs.HasKey(nameof(coolTime)) ? PlayerPrefs.GetInt(nameof(coolTime)) : 400;
        projCount = PlayerPrefs.HasKey(nameof(projCount)) ? PlayerPrefs.GetInt(nameof(projCount)) : 500;
        atkRemainTime = PlayerPrefs.HasKey(nameof(atkRemainTime)) ? PlayerPrefs.GetInt(nameof(atkRemainTime)) : 600;
        playerHealth = PlayerPrefs.HasKey(nameof(playerHealth)) ? PlayerPrefs.GetInt(nameof(playerHealth)) : 700;
        playerdef = PlayerPrefs.HasKey(nameof(playerdef)) ? PlayerPrefs.GetInt(nameof(playerdef)) : 800;
        playerMoveSpeed = PlayerPrefs.HasKey(nameof(playerMoveSpeed)) ? PlayerPrefs.GetInt(nameof(playerMoveSpeed)) : 900;
    }

    //종료 시 현재 값 PlayerPrefs에 저장
    public void UpdateAllPref()
    {
        PlayerPrefs.SetInt(nameof(atkPower), atkPower);
        PlayerPrefs.SetInt(nameof(atkScale), atkScale);
        PlayerPrefs.SetInt(nameof(projSpeed), projSpeed);
        PlayerPrefs.SetInt(nameof(coolTime), coolTime);
        PlayerPrefs.SetInt(nameof(projCount), projCount);
        PlayerPrefs.SetInt(nameof(atkRemainTime), atkRemainTime);
        PlayerPrefs.SetInt(nameof(playerHealth), playerHealth);
        PlayerPrefs.SetInt(nameof(playerdef), playerdef);
        PlayerPrefs.SetInt(nameof(playerMoveSpeed), playerMoveSpeed);
    }

    //밸류 갱신
    public void UpdatePref(string name)
    {
        switch(name)
        {
            case nameof(atkPower):
                atkPower++;
                PlayerPrefs.SetInt(nameof(atkPower), atkPower);
                break;
            case nameof(atkScale):
                atkScale++;
                PlayerPrefs.SetInt(nameof(atkScale), atkScale);
                break;
            case nameof(projSpeed):
                projSpeed++;
                PlayerPrefs.SetInt(nameof(projSpeed), projSpeed);
                break;
            case nameof(coolTime):
                coolTime++;
                PlayerPrefs.SetInt(nameof(coolTime), coolTime);
                break;
            case nameof(projCount):
                projCount++;
                PlayerPrefs.SetInt(nameof(projCount), projCount);
                break;
            case nameof(atkRemainTime):
                atkRemainTime++;
                PlayerPrefs.SetInt(nameof(atkRemainTime), atkRemainTime);
                break;
            case nameof(playerHealth):
                playerHealth++;
                PlayerPrefs.SetInt(nameof(playerHealth), playerHealth);
                break;
            case nameof(playerdef):
                playerdef++;
                PlayerPrefs.SetInt(nameof(playerdef), playerdef);
                break;
            case nameof(playerMoveSpeed):
                playerMoveSpeed++;
                PlayerPrefs.SetInt(nameof(playerMoveSpeed), playerMoveSpeed);
                break;
        }
    }

    //변수들을 배열에 담아 반환
    //강화 파라미터 추가 시 갱신 필요
    public int[] GetPrefers()
    {
        int[] arr = new int[9];

        arr[0] = atkPower;
        arr[1] = atkScale;
        arr[2] = projSpeed;
        arr[3] = coolTime;
        arr[4] = projCount;
        arr[5] = atkRemainTime;
        arr[6] = playerHealth;
        arr[7] = playerdef;
        arr[8] = playerMoveSpeed;

        return arr;
    }
}