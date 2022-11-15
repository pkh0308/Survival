using System;
using UnityEngine;

public class SpriteContainer : MonoBehaviour
{
    [SerializeField] Sprite[] sprites;

    public static Func<int, Sprite> getSprite;

    private void Awake()
    {
        getSprite = (a) => { return GetSprite(a); };
    }

    Sprite GetSprite(int id)
    {
        switch(id)
        {
            case ObjectNames.soccerBall:
                return sprites[0];
            case ObjectNames.shuriken:
                return sprites[1];
            case ObjectNames.defender:
                return sprites[2];
            default:
                return null;
        }
    }
}
