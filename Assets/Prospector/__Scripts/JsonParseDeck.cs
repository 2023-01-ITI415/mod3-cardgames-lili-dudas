using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this class stores information about each decorator or pip JSON_Deck
[System.Serializable]
public class JsonPip {
    public string type = "pip"; //pip, letter, or suit
    public Vector3 loc; //location of sprite on the card
    public bool flip = false; //true to flip the sprite vertically
    public float scale = 1; //the scale of the sprite
}

//this class stores information for each rank of card
[System.Serializable]
public class JsonCard {
    public int rank; //the rank (1-13) of this card
    public string face; //sprite to use for each face card
    public List<JsonPip> pips = new List<JsonPip>(); //the pips on this card
}

//this class contains information about the entire deck
[System.Serializable]
public class JsonDeck {
    public List<JsonPip> decorators = new List<JsonPip>();
    public List<JsonCard> cards = new List<JsonCard>();
}

public class JsonParseDeck : MonoBehaviour
{
    [Header ("Inscribed")]
    public TextAsset jsonDeckFile; //reference to the JSON_Deck text file

    [Header ("Dynamic")]
    public JsonDeck deck;

    void Awake() {
        deck = JsonUtility.FromJson<JsonDeck>(jsonDeckFile.text);
    }
}
