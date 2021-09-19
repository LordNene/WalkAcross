using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector2Int boardPosition;
    public Board board;
    public RectTransform rectTransform;

    public bool hasCard = false;

    public void Setup(int x, int y, Board newBoard)
    {
        boardPosition = new Vector2Int(x, y);
        board = newBoard;
        rectTransform = GetComponent<RectTransform>();
    }
}
