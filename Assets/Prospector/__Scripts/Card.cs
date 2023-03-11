using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    [Header("Dynamic")]
    public char suit; //suit of the card (C,D,H, or S)
    public int rank; //rank of the card (1-13)
    public Color color = Color.black; //color to tint pips
    public string colS = "Black"; //or "Red". name of Color
    public GameObject back; //the GameObject of the back of the card
    public JsonCard def; //the card layout as defined in JSON_Deck.json

    //This List holds all of the Decorator GameObjects
    public List<GameObject> decoGOs = new List<GameObject>();
    //this List holds all of the Pip GameObjects 
    public List<GameObject> pipGOs = new List<GameObject>();

    ///<summary>
    ///Creates this Card's visuals based on suit and rank.
    ///Note that this method assumes it will be passed a valid suit and rank.
    /// </summary>
    /// <param name="eSuit">The suit of the card (e.g., 'C')</param>
    /// <param name="eRank">The rank from 1 to 13</param>
    /// <returns></returns>
    public void Init(char eSuit, int eRank, bool startFaceUp=true) {
        //assign basic values to the card
        gameObject.name = name = eSuit.ToString() + eRank;
        suit = eSuit;
        rank = eRank;
        //If this is a Diamond or Heart, change the default Black color to Red
        if (suit == 'D' || suit == 'H') {
            colS = "Red";
            color = Color.red;
        }

        def = JsonParseDeck.GET_CARD_DEF(rank);
        //build the card from sprites
    }

    ///<summary>
    ///Shortcut for setting transform.localPosition
    ///</summary>
    ///<param name="v"></param>
    public virtual void SetLocalPos(Vector3 v) {
        transform.localPosition = v;
    }
}
