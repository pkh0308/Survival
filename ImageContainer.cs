using UnityEngine;
using UnityEngine.UI;

public class ImageContainer : MonoBehaviour
{
    [SerializeField] Sprite image_101;
    [SerializeField] Sprite image_201;
    [SerializeField] Sprite image_301;
    [SerializeField] Sprite image_401;
    [SerializeField] Sprite image_501;
    [SerializeField] Sprite image_601;
    [SerializeField] Sprite image_701;
    [SerializeField] Sprite image_801;
    [SerializeField] Sprite image_901;

    public Sprite GetSprite(int id)
    {
        switch(id)
        {
            case 101:
                return image_101;
            case 201:
                return image_201;
            case 301:
                return image_301;
            case 401:
                return image_401;
            case 501:
                return image_501;
            case 601:
                return image_601;
            case 701:
                return image_701;
            case 801:
                return image_801;
            case 901:
                return image_901;
            default:
                return null;
        }    
    }
}
