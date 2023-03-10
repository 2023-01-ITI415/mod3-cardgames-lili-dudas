using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class CardSpritesSO : using UnityEngine;

[CreateAssetMenu(fileName = "CardSpritesSO", menuName = "ScriptableObject/CardSpritesSO")]
public class CardSpritesSO : ScriptableObject {
    [Header("Card Stock")]
    public Sprite cardBack;
    public Sprite cardBackGold;
    public Sprite cardFront;
    public Sprite cardFrontGold;

    [Header("Suits")]
    public Sprite suitClub;
    public Sprite suitDiamond;
    public Sprite suitHeart;
    public Sprite suitSpade;

    [Header("Pip Sprites")]
    public Sprite[] faceSprites;
    public Sprite[] rankSprites;
}
