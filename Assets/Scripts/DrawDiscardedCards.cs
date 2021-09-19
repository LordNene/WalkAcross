using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class DrawDiscardedCards : NetworkBehaviour
{
    public PlayerManager Player;

    //public void OnClickDiscardPile()
    public void OnClick()
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        Player = networkIdentity.GetComponent<PlayerManager>();
        if (Player.HasPlaced || Player.HasDiscarded || !Player.IsPlayersTurn)
        {
            return;
        }
        if (Player.CurrentHandSize < PlayerManager.MaxHandSize)
        {
            int cardsToDraw = PlayerManager.MaxHandSize - Player.PlayerHandSize;
            Player.CmdDrawCards(Enums.DrawType.DiscardPile, cardsToDraw);
            //Player.CurrentHandSize++;  // TODO
            Player.HasDrawn = true;
        }
    }

    public void OnPointerEnter()
    {
        Tooltip.ShowTooltip_Static("Draw 2 cards from the discard pile");
    }

    public void OnPointerExit()
    {
        Tooltip.HideTooltip_Static();
    }
}
