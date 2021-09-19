using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class DiscardCard : NetworkBehaviour
{
    public PlayerManager Player;

    //public void OnClickDiscardCard()
    public void OnClick()
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        Player = networkIdentity.GetComponent<PlayerManager>();
        if (Player.HasDiscarded || !Player.IsPlayersTurn || !Player.HasDrawn)
        {
            return;
        }
        //Player.DiscardCard();
        var card = Player.SelectedCard;
        if (card != null)
        {
            Player.CmdDiscardCard(card);
            Player.HasDiscarded = true;
        }
    }

    public void OnPointerEnter()
    {
        Tooltip.ShowTooltip_Static("Discard selected card");
        //gameObject.GetComponent<Image>().color = new Color32(); // maybe later
    }

    public void OnPointerExit()
    {
        Tooltip.HideTooltip_Static();
    }
}
