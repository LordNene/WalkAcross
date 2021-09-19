using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    [SerializeField] public Transform tileSlotGrid;
    [SerializeField] public TileSlot[] tileSlots;

    public const int SizeX = 6;
    public const int SizeY = 6;

    void Start()
    {
        SetupTileSlots();
        for (int i = 0; i < tileSlots.Length; i++)
        {
            tileSlots[i].index = i;
            tileSlots[i].IsFilled = false;
        }
    }

    void OnValidate()
    {
        tileSlots = tileSlotGrid.GetComponentsInChildren<TileSlot>();
        for (int i = 0; i < tileSlots.Length; i++)
        {
            tileSlots[i].index = i;
            tileSlots[i].IsFilled = false;
        }
    }

    void SetupTileSlots()
    {
        int row = 0, col = 0;
        foreach (var slot in tileSlots)
        {
            int posX = row % SizeX + 1;
            int posY = col % SizeY + 1;
            slot.BoardPosition = new Vector2Int(posX, posY);
            row++;
            if (row != 0 && row % SizeX == 0)
            {
                col++;
            }
        }
    }

    public bool PutCardOnTile(GameObject card, int index)
    {
        var tile = tileSlots[index];
        if (!tile.IsFilled)
        {
            card.transform.SetParent(tile.gameObject.transform, false);
            tile.IsFilled = true;
            tile.CardColor = card.GetComponent<Card>().Color;
            return true;
        }
        return false;
    }
}
