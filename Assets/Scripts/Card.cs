using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using static Enums;
using Mirror.Websocket;

public class Card : NetworkBehaviour
{
    public int Index;
    public Vector2Int Numbers;// { get; set; }
    public CardType Type;// { get; set; }
    public Enums.Color Color;// { get; set; }
    public PlayerManager Player;

    //public Image CardBack;
    //public Image CardFront;
    public GameObject CardFrontObj;
    bool flipped = false;
    public bool Placed = false;

    public void Setup(Vector2Int numbers, CardType type, Enums.Color color)
    {
        Numbers = numbers;
        Type = type;
        Color = color;

        FindFrontCardObject();
    }

    void FindFrontCardObject()
    {
        var images = gameObject.GetComponentsInChildren<Image>();
        // first one is the card itself for some reason
        //CardBack = images[1];
        //CardFront = images[2];
        CardFrontObj = images[2].gameObject;
    }

    public void OnClick()
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        Player = networkIdentity.GetComponent<PlayerManager>();
        if (!Player.IsPlayersTurn)
        {
            return;
        }
        Player.SelectCard(gameObject);
    }

    public void Flip()
    {
        if (CardFrontObj == null) FindFrontCardObject(); // to be removed once I redo card generation
        CardFrontObj.SetActive(flipped);
        flipped = !flipped;
    }
}
