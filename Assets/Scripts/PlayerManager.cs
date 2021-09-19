using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using UnityEngine.Animations;
using Telepathy;
using System.Linq;

public class PlayerManager : NetworkBehaviour
{
    [SerializeField] GameObject[] OriginalCards;
    //CardManager cardManager; //for generating card prefabs

    public GameObject[] Cards; 
    public GameObject PlayerArea;
    public GameObject EnemyArea;
    public GameObject DiscardArea;
    public GameObject GameOverScreen;

    public GameBoard Board;
    public GameManager GameManager;

    //List<GameObject> drawPile, discardPile;
    List<GameObject> discardPile;
    //public SyncListInt drawPileIds = new SyncListInt();
    public List<int> drawPileIds = new List<int>();
    //public SyncListInt discardPileIds = new SyncListInt();
    public List<int> discardPileIds = new List<int>();

    List<GameObject> playerHand;
    public int PlayerHandSize { get { return playerHand.Count; } }

    //List<GameObject> EnemyHand;
    public GameObject SelectedCard;

    public const int MaxHandSize = 3;
    public int CurrentHandSize = 0;

    public bool IsPlayersTurn = false;
    public bool IsWinner = false;
    public Enums.PlayerColor PlayerColor;

    public bool HasDrawn = false;
    public bool HasPlaced = false;
    public bool HasDiscarded = false;

    /*private void OnDrawPileChanged(SyncListInt.Operation op, int index, int oldItem, int newItem)
    {
        Debug.Log("draw list changed " + op + " old item: " + oldItem + " new item: " + newItem);
    }

    private void OnDiscardPileChanged(SyncListInt.Operation op, int index, int oldItem, int newItem)
    {
        Debug.Log("discard list changed " + op + " old item: " + oldItem + " new item: " + newItem);
    }*/

    public override void OnStartClient()
    {
        IsWinner = false;
        base.OnStartClient();

        PlayerArea = GameObject.Find("PlayerArea");
        EnemyArea = GameObject.Find("EnemyArea");
        DiscardArea = GameObject.Find("DiscardArea");
        //GameOverScreen = GameObject.Find("GameOverScreen");

        playerHand = new List<GameObject>();
        Board = GameObject.Find("Board").GetComponent<GameBoard>();
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        discardPile = new List<GameObject>();

        RegisterPrefabs();
        ClearCardOutlines();

        SetAreaColors();
        if (isClientOnly)
        {
            PlayerColor = Enums.PlayerColor.Red;
            GameManager.UpdateTurnText();
        }
    }

    void SetAreaColors()
    {
        if (isClientOnly)
        {
            PlayerColor = Enums.PlayerColor.Red;
            PlayerArea.GetComponent<Image>().color = GameManager.Red;
            EnemyArea.GetComponent<Image>().color = GameManager.Blue;
            //IsPlayersTurn = true;
        }
        else
        {
            PlayerArea.GetComponent<Image>().color = GameManager.Blue;
            EnemyArea.GetComponent<Image>().color = GameManager.Red;
        }
    }

    void RegisterPrefabs()
    {
        Cards = Resources.LoadAll<GameObject>("Prefabs/GeneratedCards");
        foreach (var card in Cards)
        {
            ClientScene.RegisterPrefab(card);
        }
    }

    [Server]
    public override void OnStartServer()
    {
        base.OnStartServer();
        //drawPileIds.Callback += OnDrawPileChanged;
        //discardPileIds.Callback += OnDiscardPileChanged;
        IsWinner = false;
        //GenerateCards();
        Cards = Resources.LoadAll<GameObject>("Prefabs/GeneratedCards");

        for (int index = 0; index < Cards.Length - 1; index++)
        {
            Cards[index].GetComponent<Card>().Index = index; // for discarding
            Cards[index].GetComponent<Card>().Placed = false;
            drawPileIds.Add(index);
        }


        //drawPile = new List<GameObject>();
        //discardPile = new List<GameObject>();

        //drawPile.AddRange(Cards);

        IsPlayersTurn = true;
        PlayerColor = Enums.PlayerColor.Blue;
        GameManager.UpdateTurnText();
    }

    void Update()
    {
        if (NetworkManager.singleton.isNetworkActive)
        {
            PlayerManager player = NetworkClient.connection.identity.GetComponent<PlayerManager>();
            var win = GameManager.CheckVictoryConditions(player.PlayerColor);

            if (win)
            {
                IsWinner = true;
                CmdShowGameOverScreen();
            }
            /*
            if (IsWinner)
            {
                CmdShowGameOverScreen();
            }*/

            /*if (LocalPlayer == null)
            {
                FindLocalTank();
            }
            else
            {
                ShowReadyMenu();
                UpdateStats();
            }*/
        }
    }

    // checks if one of the players won at the end of one's turn (but what about draw?)
    public void EndTurn()
    {
        if (!HasPlaced || !HasDiscarded || !IsPlayersTurn) { return; }
        CmdCheckVictory();
        CmdUpdateTurns();
        HasPlaced = false;
        HasDiscarded = false;
        HasDrawn = false;
        if (SelectedCard != null)
        {
            SelectedCard.GetComponentInChildren<Outline>().enabled = false;
            SelectedCard.GetComponent<Card>().Placed = true;
        }
        ClearHighlights();
    }

    [Command]
    void CmdCheckVictory()
    {
        RpcCheckVictory();
    }

    [ClientRpc]
    void RpcCheckVictory()
    {
        PlayerManager player = NetworkClient.connection.identity.GetComponent<PlayerManager>();
        var win = GameManager.CheckVictoryConditions(player.PlayerColor);

        if (win)
        {
            IsWinner = true;
            //CmdShowGameOverScreen();
        }
    }

    [Command]
    void CmdUpdateTurns()
    {
        RpcUpdateTurns();
    }

    [ClientRpc]
    void RpcUpdateTurns()
    {
        PlayerManager player = NetworkClient.connection.identity.GetComponent<PlayerManager>();
        player.IsPlayersTurn = !player.IsPlayersTurn;
        //IsPlayersTurn = !IsPlayersTurn;
        GameManager.UpdateTurnText();
    }

    // only in editor for generating prefabs
    /*void GenerateCards()
    {
        cardManager = new CardManager(OriginalCards);
        drawPile = cardManager.GenerateCards();
        //RpcRegisterCardPrefabs();
    }*/

    [Command]
    void CmdShowGameOverScreen()
    {
        RpcShowGameOverScreen();
    }

    [ClientRpc]
    void RpcShowGameOverScreen()
    {
        var canvas = GameObject.Find("Canvas");
        var screen = Instantiate(GameOverScreen, canvas.transform.position, Quaternion.identity, canvas.transform);
        //var screen = Instantiate(GameOverScreen, new Vector3(0, 0, 0), Quaternion.identity, canvas.transform);

        //var player = NetworkClient.connection.identity.GetComponent<P>
        if (IsWinner)
        {
            screen.GetComponentInChildren<Text>().text = "Victory!";
        }
        else
        {
            screen.GetComponentInChildren<Text>().text = "Defeat.";
        }

        NetworkServer.Spawn(screen, connectionToClient);
    }

    // this should prevent player to draw 3 cards out of which none can be placed on the board
    // not very efficient I guess, could be optimized later
    [Server]
    public List<GameObject> TryToDrawCards(Enums.DrawType type, int cardsToDraw)
    {
        var drawnCards = new List<GameObject>();
        for (int i = 0; i < cardsToDraw; i++)
        {
            GameObject card;
            if (type == Enums.DrawType.DrawPile)
            {
                //if (drawPile.Count < 1) return;
                var idIndex = Random.Range(0, drawPileIds.Count);
                var cardIndex = drawPileIds[idIndex];
                card = Instantiate(Cards[cardIndex], new Vector3(0, 0, 0), Quaternion.identity);
                drawPileIds.RemoveAt(idIndex);

                //drawPileIds
                //EventDiscardPileIdRemoved?.Invoke(idIndex);
                //EventDiscardPileIdRemoved?.Invoke(idIndex);
                //RpcRemoveDrawPileAt(idIndex);
                //SyncListInt.SyncListChanged.Remove
            }
            else
            {
                //if (discardPile.Count < 1) return;

                int lastChildId = DiscardArea.transform.childCount - 1 - i; // we don't remove it here so it has to be "i"
                card = DiscardArea.transform.GetChild(lastChildId).gameObject;

                /*var idIndex = discardPileIds.Count - 1; // last card instead of random
                var cardIndex = discardPileIds[idIndex]; // last card instead of random
                card = Cards[cardIndex];
                discardPileIds.RemoveAt(idIndex);*/

                //RpcRemoveCardFromDiscardPile(cardIndex); 
            }
            drawnCards.Add(card);
        }
        return drawnCards;
    }

    [Command]
    public void CmdDrawCards(Enums.DrawType type, int cardsToDraw)
    {
        var drawnCards = new List<GameObject>();
        bool isOneOfCardsPlaceable = false;
        while (!isOneOfCardsPlaceable)
        {
            if (type == Enums.DrawType.DrawPile)
            {
                if (drawPileIds.Count < cardsToDraw) return;
            }
            else
            {
                if (discardPileIds.Count < cardsToDraw) return;
            }

            drawnCards = TryToDrawCards(type, cardsToDraw);
            /*foreach (var cardObj in drawnCards)
            {
                RpcRemoveDrawPileAt(cardObj.GetComponent<Card>().Index);
            }*/

            foreach (var tileSlot in Board.tileSlots)
            {
                foreach (var cardObj in drawnCards)
                {
                    Card card = cardObj.GetComponent<Card>();
                    if (card != null && !tileSlot.IsFilled && IsTileAvailable(card, tileSlot.BoardPosition))
                    {
                        isOneOfCardsPlaceable = true;
                        break;
                    }
                }
            }
            if (!isOneOfCardsPlaceable)
            {
                // if none of the cards can be placed, we could return it to the deck, but let's just get rid of them to make things easier
                drawnCards.Clear();
            }
        }

        foreach (var card in drawnCards)
        { 
            NetworkServer.Spawn(card, connectionToClient);
            RpcShowDealtCard(card);
            card.GetComponent<Card>().Placed = false; // just in case
            RpcAddCardToPlayerHand(card);

            // think we should be able to draw until hand is full
            //if (type == Enums.DrawType.DiscardPile) { return; } // how much cards can you take from DiscardPile?
        }
    }

    [ClientRpc]
    void RpcAddCardToPlayerHand(GameObject card)
    {
        if (hasAuthority) playerHand.Add(card);
    }

    [Client]
    public void SelectCard(GameObject cardObject)
    {
        if (!IsPlayersTurn) { return; }
        if (SelectedCard != null)
        {
            SelectedCard.GetComponentInChildren<Outline>().enabled = false;
        }

        SelectedCard = cardObject;
        CmdSetSelectedCard(cardObject); // weird pattern, but hotfix for making card.Flip work
        Card card = SelectedCard.GetComponent<Card>();
        if (card.Placed)
        {
            Debug.Log("Player already placed a card");
            return;
        }

        var outline = cardObject.GetComponentInChildren<Outline>();
        outline.enabled = true;

        HighlightAvailableTiles(card);
    }

    [Command]
    void CmdSetSelectedCard(GameObject card)
    {
        RpcSetSelectedCard(card);
    }

    [ClientRpc]
    void RpcSetSelectedCard(GameObject card)
    {
        SelectedCard = card;
    }

    void HighlightAvailableTiles(Card card)
    {
        foreach (var tileSlot in Board.tileSlots)
        {
            Image image = tileSlot.gameObject.GetComponent<Image>();
            if (card != null && !tileSlot.IsFilled && IsTileAvailable(card, tileSlot.BoardPosition))
            {
                image.color = new Color32(255, 255, 0, 150);
            }
            else
            {
                image.color = new Color32(255, 255, 255, 0);
            }
        }
    }

    void ClearHighlights()
    {
        HighlightAvailableTiles(null); // clears all tiles
    }

    void ClearCardOutlines()
    {
        foreach (var card in Cards)
        {
            card.GetComponentInChildren<Outline>().enabled = false;
        }
    }

    bool IsTileAvailable(Card card, Vector2Int tilePosition)
    {
        switch (card.Type)
        {
            case Enums.CardType.Classic:
                return card.Numbers == tilePosition || new Vector2Int(card.Numbers.y, card.Numbers.x) == tilePosition;
            case Enums.CardType.Diagonal:
                return card.Numbers == tilePosition;
            case Enums.CardType.Super:
                return card.Numbers.x == tilePosition.x || card.Numbers.x == tilePosition.y;
            default:
                return false;
        }
    }

    public void SelectTile(GameObject tileSlot)
    {
        if (HasPlaced || !IsPlayersTurn) { return; }
        if (SelectedCard != null)
        {
            Card card = SelectedCard.GetComponent<Card>();
            var slot = tileSlot.GetComponent<TileSlot>();
            if (slot.IsFilled)
            {
                return;
            }
            if (IsTileAvailable(card, slot.BoardPosition))
            {
                CmdPlaceCard(SelectedCard, slot.index, card.Color, slot.BoardPosition);

                SelectedCard.GetComponentInChildren<Outline>().enabled = false;
                SelectedCard = null;

                //EndTurn();
            }
        }
    }

    [Command]
    public void CmdPlaceCard(GameObject card, int index, Enums.Color color, Vector2Int boardPosition)
    {
        RpcPlaceCard(card, index, color, boardPosition);
    }

    [ClientRpc]
    void RpcPlaceCard(GameObject card, int index, Enums.Color color, Vector2Int boardPosition)
    {
        var placed = Board.PutCardOnTile(card, index);
        if (placed)
        {
            GameManager.MarkPlacedCard(color, boardPosition);
            ClearHighlights();

            if (hasAuthority)
            {
                playerHand.Remove(card);
            }
            if (!hasAuthority) 
            {
                card.GetComponent<Card>().Flip();
            }

        }
    }

    [ClientRpc]
    void RpcShowDealtCard(GameObject card)
    {
        if (hasAuthority)
        {
            card.transform.SetParent(PlayerArea.transform, false);
            //card.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        else
        {
            card.transform.SetParent(EnemyArea.transform, false);
            card.GetComponent<Card>().Flip();
            //drawPileIds.RemoveAt(card.GetComponent<Card>().Index);
            //card.transform.Rotate(new Vector3(0,180));
        }
    }
    /*
    public void DiscardCard()
    {
        if (HasDiscarded || !IsPlayersTurn) { return; }
        if (SelectedCard != null)
        {
            CmdDiscardCard(SelectedCard);
        }
    }*/

    [Command]
    public void CmdDiscardCard(GameObject card)
    {
        //discardPile.Add(card); // for some reason adding it here means only the side who discarded the card can later draw it --> I guess synclists?
        discardPileIds.Add(card.GetComponent<Card>().Index);
        RpcPutCardIntoDiscardArea(card);
    }

    [ClientRpc]
    void RpcPutCardIntoDiscardArea(GameObject card)
    {
        card.GetComponent<Card>().Placed = true; // so that it's not clickable in the discard area
        if (hasAuthority)
        {
            SelectedCard.GetComponentInChildren<Outline>().enabled = false;
            //SelectedCard.GetComponent<Card>().Placed = true; // so that it's not clickable in the discard area
            //SelectedCard = null;
            ClearHighlights();
            playerHand.Remove(card);
        }
        if (!hasAuthority)
        {
            card.GetComponent<Card>().Flip();
        }

        //discardPile.Add(card);
        discardPileIds.Add(card.GetComponent<Card>().Index);
        card.transform.SetParent(DiscardArea.transform, true);
    }
}
