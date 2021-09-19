using Mirror;
using Mirror.Cloud.Examples.Pong;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSlot : NetworkBehaviour
{
    public Vector2Int BoardPosition;
    //public GameObject Card { get; set; }
    public int index;
    public bool IsFilled { get; set; } = false;
    public Enums.Color CardColor;
    public PlayerManager Player;

    public void OnClick()
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        Player = networkIdentity.GetComponent<PlayerManager>();
        if (Player.HasPlaced || !Player.IsPlayersTurn)
        {
            return;
        }
        Player.SelectTile(gameObject);
        Player.HasPlaced = true;
    }
}
