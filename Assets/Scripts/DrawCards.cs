using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class DrawCards : NetworkBehaviour
{
    public PlayerManager Player;
    /*public Button DrawPileButton;
    public Button DiscardPileButton;
    public Button DiscardCardButton;

    void Start()
    {
        DrawPileButton = GameObject.FindGameObjectWithTag("DrawPileButton").GetComponent<Button>();
        DiscardPileButton = GameObject.FindGameObjectWithTag("DiscardPileButton").GetComponent<Button>();
        DiscardCardButton = GameObject.FindGameObjectWithTag("DiscardCardButton").GetComponent<Button>();

        DrawPileButton.onClick.AddListener(OnClickDrawPile);
        DiscardPileButton.onClick.AddListener(OnClickDiscardPile);
        DiscardPileButton.onClick.AddListener(OnClickDiscardCard);
    }*/

    //public void OnClickDrawPile()
    public void OnClick()
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        Player = networkIdentity.GetComponent<PlayerManager>();

        if (Player.HasDrawn || Player.HasPlaced || Player.HasDiscarded || !Player.IsPlayersTurn) 
        { 
            return; 
        }
        int cardsToDraw = PlayerManager.MaxHandSize - Player.PlayerHandSize;
        Player.CmdDrawCards(Enums.DrawType.DrawPile, cardsToDraw);
        Player.HasDrawn = true;
        //Player.CurrentHandSize = 3;
    }

    public void OnPointerEnter()
    {
        Tooltip.ShowTooltip_Static("Draw up to 3 cards from the draw pile");
    }

    public void OnPointerExit()
    {
        Tooltip.HideTooltip_Static();
    }
}
