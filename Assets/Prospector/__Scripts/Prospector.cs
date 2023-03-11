using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Deck))]
[RequireComponent(typeof(JsonParseLayout))]

public class Prospector : MonoBehaviour
{
    private static Prospector S; //a private Singleton for prospector

    [Header("Dynamic")]
    public List<CardProspector> drawPile;
    public List<CardProspector> discardPile;
    public List<CardProspector> mine;
    public CardProspector target;

    private Transform layoutAnchor;
    private Deck deck;
    private JsonLayout jsonLayout;
    
    void Start()
    {
        //set the private SIngleton. 
        if (S != null) Debug.LogError("Attempted to set S more than once!");
        S = this;
        jsonLayout = GetComponent<JsonParseLayout>().layout;

        deck = GetComponent<Deck>();
        //these two lines replace the start() call we commeneted out in deck
        deck.InitDeck();
        Deck.Shuffle(ref deck.cards);

        drawPile = ConvertCardsToCardsProspectors(deck.cards);
        LayoutMine();
    }

    ///<summary>
    ///Converts each Card in a List(Card) into a List(CardProspector) so that it
    ///can be used in the Prospector game. 
    ///</summary>
    ///<param name="listedCard">A List(Card) to be converted</param>
    ///<returns>A List(Cardprospector) of the converted cards</returns>
    List<CardProspector> ConvertCardsToCardsProspectors(List<Card> listCard) {
        List<CardProspector> listCP = new List<CardProspector>();
        CardProspector cp;
        foreach( Card card in listCard) {
            cp = card as CardProspector;
            listCP.Add(cp);
        }
        return(listCP);
    }

    ///<summary>
    ///Pulls a single card from the beginnning of the drawPile and returns it
    ///Note: there is no protection against trying to draw from an empty pile!
    ///</summary>
    ///<returns>The top card of drawPile</returns>
    CardProspector Draw() {
        CardProspector cp = drawPile[0]; //Pull the 0th CardProtector
        drawPile.RemoveAt(0); //then remove it from drawPile
        return(cp); //and return it
    }

    ///<summary>
    ///Positions the intial tableau of cards, a.k.a the "mine"
    ///</summary>
    void LayoutMine () {
        //create an empty gameObject to serve as an anchor for the tableau
        if(layoutAnchor == null) {
            //create an empty GameObject named _LayoutAnchor in the Hierarchy
            GameObject tGO = new GameObject("_LayoutAnchor");
            layoutAnchor = tGO.transform; //grab its Transform
        }
        CardProspector cp;

        //iterate through JsonLayoutSlots pulled from the JSON_Layout 
        foreach (JsonLayoutSlot slot in jsonLayout.slots) {
            cp = Draw(); //pull a card from the top of the draw Pile
            cp.faceUp = slot.faceUp;
            //make the CardProspector a child of layoutAnchor
            cp.transform.SetParent(layoutAnchor);

            //convert the last char of the layer string to an int 
            int z = int.Parse(slot.layer[slot.layer.Length - 1].ToString());

            //set the localPosition of the card based on the slot information
            cp.SetLocalPos( new Vector3 (
                jsonLayout.multiplier.x * slot.x,
                jsonLayout.multiplier.y * slot.y,
                - z));
            cp.layoutID = slot.id;
            cp.layoutSlot = slot;
            //CardProspectors in the mine have the state cardState.mine
            cp.state = eCardState.mine;

            mine.Add(cp);//Add this CardProspector to the List<> mine
        }
    }
}
