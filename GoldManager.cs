using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldManager
{
    // 생성자 및 소멸자
    private GoldManager()
    {
        gold = PlayerPrefs.HasKey(nameof(gold)) ? PlayerPrefs.GetInt(nameof(gold)) : 0;
    }
    ~GoldManager()
    {
        PlayerPrefs.SetInt(nameof(gold), gold);
    }

    // 싱글톤 구현
    private static GoldManager instance;
    public static GoldManager Instance
    {
        get
        {
            if (instance == null)
                instance = new GoldManager();

            return instance;
        }
    }

    int gold;
    public int Gold { get { return gold; } }

    //외부 사용 함수
    public void PlusGold(int plus)
    {
        gold += plus;
        PlayerPrefs.SetInt(nameof(gold), gold);
    }

    public bool Purchase(int price)
    {
        if (price > gold) return false;

        gold -= price;
        PlayerPrefs.SetInt(nameof(gold), gold);

        return true;
    }
}