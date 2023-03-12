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

        //set up the initial target card
        MoveToTarget ( Draw () );

        //set up the draw pile
        UpdateDrawPile();
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

            //set the sorting layer of all SpriteRenderers on the Card
            cp.SetSpriteSortingLayer(slot.layer);
            mine.Add(cp);//Add this CardProspector to the List<> mine
        }
    }

    ///<summary>
    ///Moves the current target card to the discardPile
    ///</summary>
    ///<param name="cp">The CardProspector to be moved</param>
    void MoveToDiscard(CardProspector cp) {
        //set the state of the card to discard
        cp.state = eCardState.discard;
        discardPile.Add(cp); //Add it to the discardPile List<>
        cp.transform.SetParent(layoutAnchor); //Update its transform parent

        //position it on the discardPile
        cp.SetLocalPos(new Vector3(
            jsonLayout.multiplier.x * jsonLayout.discardPile.x,
            jsonLayout.multiplier.y * jsonLayout.discardPile.y,
            0));
        cp.faceUp = true;

        //Place it on top of the pile for depth sorting
        cp.SetSpriteSortingLayer(jsonLayout.discardPile.layer);

        cp.SetSortingOrder(-200 + (discardPile.Count * 3));
    }

    ///<summary>
    ///make cp the new target card
    ///</summary>
    ///<param name="cp">The CardProspector to be moved</param>
    void MoveToTarget(CardProspector cp){
        //if the target is currently a target card, move it to discardPile
        if (target != null) MoveToDiscard(target);

        //use MoveToDiscard to move the target card to the correct location
        MoveToDiscard(cp);

        //then set a few additional things to make cp the new target
        target = cp; //cp is the new target
        cp.state = eCardState.target;

        //set the depth sorting so that cp is on top of the discardPile
        cp.SetSpriteSortingLayer("Target");

        cp.SetSortingOrder(0);
    }

    ///<summary>
    ///Arranges all the cards of the drawPile to show how many are left
    ///</summary>
    void UpdateDrawPile() {
        CardProspector cp;
        //go through all the cards of the drawPile
        for (int i = 0; i < drawPile.Count; i++) {
            cp = drawPile[i];
            cp.transform.SetParent(layoutAnchor);

            //position it correctly with the layout.drawPile.stagger
            Vector3 cpPos = new Vector3();
            cpPos.x = jsonLayout.multiplier.x * jsonLayout.drawPile.x;
            //add the staggering for the drawPile
            //cpPos.x += jsonLayout.drawPile.xStagger * i;
            cpPos.y = jsonLayout.multiplier.y * jsonLayout.drawPile.y;
            cpPos.z = 0.1f * i;
            cp.SetLocalPos(cpPos);

            cp.faceUp = false; //DrawPile Cards are all face-down
            cp.state = eCardState.drawpile;
            //set depth sorting
            cp.SetSpriteSortingLayer(jsonLayout.drawPile.layer);
            cp.SetSortingOrder(-10 * i);
        }
    }

    ///<summary>
    ///Handler for any time a card in the game is clicked
    ///</summary>
    ///<param name="cp">The CardProspector that was clicked</param>
    static public void CARD_CLICKED(CardProspector cp) {
        //the reaction is determined by the state of the clicked card
        switch (cp.state) {
            case eCardState.target:
            //clicking the target card does nothing
            break;
            case eCardState.drawpile:
            //clicking *any* card in the drawPile will draw the next card 
            //call the two methods on the Prospector Singleton S
            S.MoveToTarget(S.Draw()); //Draw a new target card
            S.UpdateDrawPile();
            break;
            case eCardState.mine:
            //clicking a card in the mine will check if it's a valid play
            bool validMatch = true; //Initially assumes that it's valid

            //if the card is face-down, it's not valid
            if (!cp.faceUp) validMatch = false;

            //if it's not an adjacent rank, it's not valid
            if (!cp.AdjacentTo(S.target)) validMatch = false;

            if(validMatch) {
                S.mine.Remove(cp); //remove it from the tableau List
                S.MoveToTarget(cp); //Make it the target card
            }
            break;
        }
    }
}
