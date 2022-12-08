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
    int cooltime;
    public int Cooltime { get { return cooltime; } }
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
        cooltime = PlayerPrefs.HasKey(nameof(cooltime)) ? PlayerPrefs.GetInt(nameof(cooltime)) : 400;
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
        PlayerPrefs.SetInt(nameof(cooltime), cooltime);
        PlayerPrefs.SetInt(nameof(projCount), projCount);
        PlayerPrefs.SetInt(nameof(atkRemainTime), atkRemainTime);
        PlayerPrefs.SetInt(nameof(playerHealth), playerHealth);
        PlayerPrefs.SetInt(nameof(playerdef), playerdef);
        PlayerPrefs.SetInt(nameof(playerMoveSpeed), playerMoveSpeed);
    }

    //밸류 갱신
    public void UpdatePref(string type)
    {
        switch(type)
        {
            case ObjectNames.atkPowerName:
                atkPower++;
                PlayerPrefs.SetInt(nameof(atkPower), atkPower);
                break;
            case ObjectNames.atkScaleName:
                atkScale++;
                PlayerPrefs.SetInt(nameof(atkScale), atkScale);
                break;
            case ObjectNames.projSpeedName:
                projSpeed++;
                PlayerPrefs.SetInt(nameof(projSpeed), projSpeed);
                break;
            case ObjectNames.cooltimeName:
                cooltime++;
                PlayerPrefs.SetInt(nameof(cooltime), cooltime);
                break;
            case ObjectNames.projCountName:
                projCount++;
                PlayerPrefs.SetInt(nameof(projCount), projCount);
                break;
            case ObjectNames.atkRemainTimeName:
                atkRemainTime++;
                PlayerPrefs.SetInt(nameof(atkRemainTime), atkRemainTime);
                break;
            case ObjectNames.playerHealthName:
                playerHealth++;
                PlayerPrefs.SetInt(nameof(playerHealth), playerHealth);
                break;
            case ObjectNames.playerDefName:
                playerdef++;
                PlayerPrefs.SetInt(nameof(playerdef), playerdef);
                break;
            case ObjectNames.playerMoveSpeedName:
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
        arr[3] = cooltime;
        arr[4] = projCount;
        arr[5] = atkRemainTime;
        arr[6] = playerHealth;
        arr[7] = playerdef;
        arr[8] = playerMoveSpeed;

        return arr;
    }

    public void Reset()
    {
        atkPower = 100;
        atkScale = 200;
        projSpeed = 300;
        cooltime = 400;
        projCount = 500;
        atkRemainTime = 600;
        playerHealth = 700;
        playerdef = 800;
        playerMoveSpeed = 900;

        PlayerPrefs.SetInt(nameof(atkPower), atkPower);
        PlayerPrefs.SetInt(nameof(atkScale), atkScale);
        PlayerPrefs.SetInt(nameof(projSpeed), projSpeed);
        PlayerPrefs.SetInt(nameof(cooltime), cooltime);
        PlayerPrefs.SetInt(nameof(projCount), projCount);
        PlayerPrefs.SetInt(nameof(atkRemainTime), atkRemainTime);
        PlayerPrefs.SetInt(nameof(playerHealth), playerHealth);
        PlayerPrefs.SetInt(nameof(playerdef), playerdef);
        PlayerPrefs.SetInt(nameof(playerMoveSpeed), playerMoveSpeed);
    }
}