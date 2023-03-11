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
        AddDecorators();
        AddPips();
        AddFace();
    }

    ///<summary>
    ///Shortcut for setting transform.localPosition
    ///</summary>
    ///<param name="v"></param>
    public virtual void SetLocalPos(Vector3 v) {
        transform.localPosition = v;
    }

    //These private variables that will be reused several times
    private Sprite _tSprite = null;
    private GameObject _tGO = null;
    private SpriteRenderer _tSRend = null;

    //An Euler rotation of 180 degrees around the Z-axis will flip sprites upside
    private Quaternion _flipRot = Quaternion.Euler(0, 0, 180);

    /// <summary>
    /// Adds the decorators to the top-left and bottom-right of each card.
    /// Deocrators are the suit and rank in the corners of each card.
    /// </summary>
    private void AddDecorators() {
        //add Decorators 
        foreach (JsonPip pip in JsonParseDeck.DECORATORS){
            if (pip.type == "suit") {
                //instantiate a Sprite GameObject
                _tGO = Instantiate<GameObject>(Deck.SPRITE_PREFAB,transform);
                //get the SpriteRenderer Component
                _tSRend = _tGO.GetComponent<SpriteRenderer>();
                //get the suit Sprite from the CardSpritesSO.SUIT static field
                _tSRend.sprite = CardSpritesSO.SUITS[suit];
            }
            else {
                _tGO = Instantiate<GameObject>(Deck.SPRITE_PREFAB,transform);
                _tSRend = _tGO.GetComponent<SpriteRenderer>();
                //get the rank Sprite from the CardSpritesSO.RANK static field
                _tSRend.sprite = CardSpritesSO.RANKS[rank];
                //set the color of the rank to match the suit
                _tSRend.color = color;
            }
            //make the Deocrator Sprites render above the Card
            _tSRend.sortingOrder = 1;

            //set the localPosition based on the location form DeckXML
            _tGO.transform.localPosition = pip.loc;
            //Flip the deocrator if needed
            if (pip.flip) _tGO.transform.rotation = _flipRot;
            //set the sclae to keep deocrators from being too big 
            if (pip.scale != 1) {
                _tGO.transform.localScale = Vector3.one * pip.scale;
            }
            //name this GameObject so it's easy to find in the Hierarchy
            _tGO.name = pip.type;
            //Add this decorator GameObject to the List card.decoGOs
            decoGOs.Add(_tGO);
        }
    }

    /// <summary>
    /// Adds pips to the front of all cards from A to 10
    /// </summary>
    private void AddPips() {
        int pipNum = 0;
        //For each of the pips in the definition...
        foreach (JsonPip pip in def.pips) {
            //instantiate a GameObject from the Deck.SPRITE_PREFAB static field
            _tGO = Instantiate<GameObject>(Deck.SPRITE_PREFAB, transform);
            //set the position to that specified in the XML
            _tGO.transform.localPosition = pip.loc;
            //Flip it if necessary
            if (pip.flip) _tGO.transform.rotation = _flipRot;
            //scale it if necessary (only for Ace)
            if (pip.scale != 1) {
                _tGO.transform.localScale = Vector3.one * pip.scale;
            }
            //give this GameObject a name
            _tGO.name = "pip_"+pipNum++;
            //get the SpriteRenderer Component
            _tSRend = _tGO.GetComponent<SpriteRenderer>();
            //set the Sprite to the proper suit
            _tSRend.sprite = CardSpritesSO.SUITS[suit];
            //sortingOrder=1 renders this pip above the Card_Front
            _tSRend.sortingOrder = 1;
            //add this to the Card's list of pips
            pipGOs.Add(_tGO);
        }
    }

    /// <summary>
    /// Adds the face sprite for the card ranks 11 to 13
    /// </summary>
    private void AddFace() {
        if (def.face == "")
            return; //no need to run if this isn't a face card
        //find a face sprite in CardSpritesSO with the right name
        string faceName = def.face + suit;
        _tSprite = CardSpritesSO.GET_FACE( faceName);
        if ( _tSprite == null) {
            Debug.LogError("Face sprite " + faceName + " not found." );
            return;
        }
        _tGO = Instantiate<GameObject>( Deck.SPRITE_PREFAB, transform );
        _tSRend = _tGO.GetComponent<SpriteRenderer>();
        _tSRend.sprite = _tSprite; //assign the face Sprite to _tSRend
        _tSRend.sortingOrder = 1; //set the sortingOrder
        _tGO.transform.localPosition = Vector3.zero;
        _tGO.name = faceName;
    }
}
