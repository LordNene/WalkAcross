using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class GameManager : NetworkBehaviour
{
    public PlayerManager Player;
    Text TurnText;
    GameBoard GameBoard;
    //public List<string> printed = new List<string>(); // debug

    public Color Red = new Color32(160, 61, 63, 255);
    public Color Blue = new Color32(63, 75, 160, 255);

    class BoardUnit
    {
        public BoardUnit Left, Right, Up, Down;
        public int Value, X, Y;
        public bool Visited = false;
        public override string ToString() //debug
        {
            if (Visited) return "V";
            return "_";
            //return Value.ToString();
        }
    }
    BoardUnit[,] BoardMatrix;

    void Start()
    {
        //TurnText = GameObject.FindGameObjectWithTag("TurnText").GetComponent<Text>();

        BoardMatrix = new BoardUnit[GameBoard.SizeX, GameBoard.SizeY];
        InitBoardMatrix();
    }

    void InitBoardMatrix()
    {
        for (int x = 0; x < GameBoard.SizeX; x++)
        {
            for (int y = 0; y < GameBoard.SizeY; y++)
            {
                BoardMatrix[x, y] = new BoardUnit();
            }
        }

        GameBoard = GameObject.Find("Board").GetComponent<GameBoard>();

        foreach (var tileSlot in GameBoard.tileSlots)
        {
            int x = tileSlot.BoardPosition.x - 1;
            int y = tileSlot.BoardPosition.y - 1;

            var tile = BoardMatrix[x, y];
            tile.Value = 0;
            tile.X = x;
            tile.Y = y;

            if (y - 1 > 0) tile.Left = BoardMatrix[x, y - 1];
            if (y + 1 < GameBoard.SizeY) tile.Right = BoardMatrix[x, y + 1];
            if (x - 1 > 0) tile.Down = BoardMatrix[x - 1, y];
            if (x + 1 < GameBoard.SizeX) tile.Up = BoardMatrix[x + 1, y];
        }

        /*for (int x = 0; x < GameBoard.SizeX; x++)
        {
            for (int y = 0; y < GameBoard.SizeY; y++)
            {
                var tile = BoardMatrix[x, y];
                tile.Value = 0;
                tile.X = x;
                tile.Y = y;
                if (x - 1 > 0) tile.Left = BoardMatrix[x - 1, y];
                if (y - 1 > 0) tile.Down = BoardMatrix[x, y - 1];
                if (x + 1 < GameBoard.SizeX) tile.Right = BoardMatrix[x + 1, y];
                if (y + 1 < GameBoard.SizeY) tile.Up = BoardMatrix[x, y + 1];
            }
        }*/
    }

    // just for debugging
    List<string> PrintMatrix()
    {
        var list = new List<string>();
        for (int x = 0; x < GameBoard.SizeX; x++)
        {
            var output = "";
            for (int y = 0; y < GameBoard.SizeY; y++)
            {
                //output += "[" + x + "," + y + "] = ";
                output += BoardMatrix[x, y].ToString() + "." + BoardMatrix[x, y].Value.ToString() + " ";
            }
            list.Add(output);
        }
        //list.Reverse();
        return list;
    }

    void ClearVisited()
    {
        for (int x = 0; x < GameBoard.SizeX; x++)
        {
            for (int y = 0; y < GameBoard.SizeY; y++)
            {
                BoardMatrix[x, y].Visited = false;
            }
        }
    }

    public void UpdateTurnText()
    {
        TurnText = GameObject.FindGameObjectWithTag("TurnText").GetComponent<Text>(); //TODO try if it works in start
        Player = NetworkClient.connection.identity.GetComponent<PlayerManager>();

        if (Player.IsPlayersTurn)
        {
            TurnText.text = "Your turn";
            
        }
        else
        {
            TurnText.text = "Enemy turn";
        }
        Player.HasDiscarded = false;
        Player.HasDrawn = false;
        Player.HasPlaced = false;

        TurnText.color = ColorText(Player.PlayerColor);
    }

    Color ColorText(Enums.PlayerColor color)
    {
        switch(color)
        {
            case Enums.PlayerColor.Red:
                return Red;
            case Enums.PlayerColor.Blue:
                return Blue;
            default:
                return Blue;
        }
    }

    // Marks that card was placed in BoardMatrix
    // 1 marks (Blue) player1
    // 2 marks (Red) player2
    // 3 marks diagonal & super cards which work for both players
    // defaultly set to 0 during initialization
    // boardPosition is from 1 to SizeX
    public void MarkPlacedCard(Enums.Color color, Vector2Int boardPosition)
    {
        int value = 0;
        switch (color)
        {
            case Enums.Color.Blue:
                //BoardMatrix[boardPosition.x - 1, boardPosition.y - 1].Value = 1;
                value = 1;
                break;
            case Enums.Color.Red:
                //BoardMatrix[boardPosition.x - 1, boardPosition.y - 1].Value = 2;
                value = 2;
                break;
            case Enums.Color.Both:
            case Enums.Color.Super:
                //BoardMatrix[boardPosition.x - 1, boardPosition.y - 1].Value = 3;
                value = 3;
                break;
        }
        //CmdMarkCardOnBoard(boardPosition.x - 1, boardPosition.y - 1, value);

        BoardMatrix[boardPosition.x - 1, boardPosition.y - 1].Value = value;
        //printed = PrintMatrix();
    }

    // assumes SizeX=SizeY
    // assumes player1 tries to connect up and bottom, and player2 left and right
    public bool CheckVictoryConditions(Enums.PlayerColor color)
    {
        ClearVisited();
        if (color == Enums.PlayerColor.Blue) //player1
        {
            for (int i = 0; i < GameBoard.SizeX; i++)
            {
                if (IsPathBottomTop(BoardMatrix[i, 0])) return true;
                
            }
        }
        else
        {
            for (int i = 0; i < GameBoard.SizeX; i++)
            {
                if (IsPathLeftRight(BoardMatrix[0, i])) return true;
            }
        }
        return false;
    }

    bool IsPathBottomTop(BoardUnit currentTile)
    {
        if (currentTile == null || currentTile.Value == 2 || currentTile.Value == 0 || currentTile.Visited) return false;
        currentTile.Visited = true;
        if (currentTile.Y == GameBoard.SizeX - 1 && (currentTile.Value == 1 || currentTile.Value == 3)) return true;
        return IsPathBottomTop(currentTile.Left) || IsPathBottomTop(currentTile.Right)
            || IsPathBottomTop(currentTile.Down) || IsPathBottomTop(currentTile.Up);
    }

    bool IsPathLeftRight(BoardUnit currentTile)
    {
        if (currentTile == null || currentTile.Value == 1 || currentTile.Value == 0 || currentTile.Visited) return false;
        currentTile.Visited = true;
        if (currentTile.X == GameBoard.SizeY - 1 && (currentTile.Value == 2 || currentTile.Value == 3)) return true;
        return IsPathLeftRight(currentTile.Left) || IsPathLeftRight(currentTile.Right)
            || IsPathLeftRight(currentTile.Down) || IsPathLeftRight(currentTile.Up);
    }
}
