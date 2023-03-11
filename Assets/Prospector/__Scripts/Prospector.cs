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
}
