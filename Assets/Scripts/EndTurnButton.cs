using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class EndTurnButton : NetworkBehaviour
{
    public PlayerManager Player;

    public void OnClick()
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        Player = networkIdentity.GetComponent<PlayerManager>();
        if (!Player.HasPlaced || !Player.HasDiscarded || !Player.IsPlayersTurn)
        {
            return;
        }
        Player.EndTurn();
    }

    public void OnPointerEnter()
    {
        Tooltip.ShowTooltip_Static("End your turn");
    }

    public void OnPointerExit()
    {
        Tooltip.HideTooltip_Static();
    }
}
