using System;
using UnityEngine;

//UI 이미지 갱신용 저장소
public class SpriteContainer : MonoBehaviour
{
    [Header("무기")]
    [SerializeField] Sprite soccerBall;
    [SerializeField] Sprite shuriken;
    [SerializeField] Sprite defender;
    [SerializeField] Sprite missile;
    [SerializeField] Sprite thunder;
    [SerializeField] Sprite explodeMine;

    [Header("업그레이드 무기")]
    [SerializeField] Sprite quantumBall; 
    [SerializeField] Sprite shadowEdge;
    [SerializeField] Sprite guardian;
    [SerializeField] Sprite sharkMissile;
    [SerializeField] Sprite judgement;
    [SerializeField] Sprite hellfireMine;

    [Header("무기(Extra Option)")]
    [SerializeField] Sprite meat_50;
    [SerializeField] Sprite gold_70;

    [Header("악세사리")]
    [SerializeField] Sprite acc_bullet;
    [SerializeField] Sprite acc_fuel;
    [SerializeField] Sprite acc_accelerator;
    [SerializeField] Sprite acc_energyCube;
    [SerializeField] Sprite acc_extraBullet;
    [SerializeField] Sprite acc_exoskeleton;
    [SerializeField] Sprite acc_heart;
    [SerializeField] Sprite acc_armor;
    [SerializeField] Sprite acc_sneakers;

    [Header("기타 UI")]
    [SerializeField] Sprite weaponSlotNormal;
    [SerializeField] Sprite weaponSlotLegandary;
    [SerializeField] Sprite blackStar;
    [SerializeField] Sprite normalStar;
    [SerializeField] Sprite legandaryStar;

    public static Func<int, Sprite> getSprite;

    private void Awake()
    {
        getSprite = (a) => { return GetSprite(a); };
    }

    Sprite GetSprite(int id)
    {
        switch(id)
        {
            //무기
            case ObjectNames.soccerBall:
                return soccerBall;
            case ObjectNames.shuriken:
                return shuriken;
            case ObjectNames.defender:
                return defender;
            case ObjectNames.missile:
                return missile;
            case ObjectNames.thunder:
                return thunder;
            case ObjectNames.explodeMine:
                return explodeMine;
            case ObjectNames.meat_50:
                return meat_50;
            case ObjectNames.gold_70:
                return gold_70;
            //악세사리
            case ObjectNames.acc_bullet:
                return acc_bullet;
            case ObjectNames.acc_fuel:
                return acc_fuel;
            case ObjectNames.acc_accelerator:
                return acc_accelerator;
            case ObjectNames.acc_energyCube:
                return acc_energyCube;
            case ObjectNames.acc_extraBullet:
                return acc_extraBullet;
            case ObjectNames.acc_exoskeleton:
                return acc_exoskeleton;
            case ObjectNames.acc_heart:
                return acc_heart;
            case ObjectNames.acc_armor:
                return acc_armor;
            case ObjectNames.acc_sneakers:
                return acc_sneakers;
            //업그레이드 무기
            case ObjectNames.quantumBall:
                return quantumBall;
            case ObjectNames.shadowEdge:
                return shadowEdge;
            case ObjectNames.guardian:
                return guardian;
            case ObjectNames.sharkMissile:
                return sharkMissile;
            case ObjectNames.judgement:
                return judgement;
            case ObjectNames.hellfireMine:
                return hellfireMine;
            //기타 UI
            case ObjectNames.weaponSlotNormal:
                return weaponSlotNormal;
            case ObjectNames.weaponSlotLegandary:
                return weaponSlotLegandary;
            case ObjectNames.blackStar:
                return blackStar;
            case ObjectNames.normalStar:
                return normalStar;
            case ObjectNames.legandaryStar:
                return legandaryStar;
            default:
                return null;
        }
    }
}
