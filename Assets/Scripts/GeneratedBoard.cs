using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public Camera MainCamera;

    public const int SizeX = 8;
    public const int SizeY = 8;
    public const float Spacing = 70;

    public GameObject tilePrefab;
    public Tile[,] tiles;

    public void Start()
    {
        MainCamera.orthographicSize = (float)SizeX / 2 + 2;
        //var rectTransform = GetComponent<RectTransform>();
        //rectTransform.sizeDelta = new Vector2(SizeX * 64, SizeY * 64);

        tiles = new Tile[SizeX, SizeY];

        //GenerateBoard();
    }

    public void GenerateBoard()
    {
        for (int x = 0; x < SizeX; x++)
        {
            for (int y = 0; y < SizeY; y++)
            {
                // Setup all tiles
                tiles[x, y] = GenerateTile(x, y);
                //tiles[x, y].Setup(x, y, this); //, this
            }
        }
    }

    public Tile GenerateTile(int x, int y)
    {
        var position = Vector3.right * x * Spacing + Vector3.up * y * Spacing;
        var parent = GameObject.FindGameObjectWithTag("Board").transform;
        GameObject spawnedTile = Instantiate(tilePrefab, position, Quaternion.identity, parent);
        //GameObject spawnedTile = Instantiate(tilePrefab, transform, parent);

        //float posX = x * 1 + Spacing;
        //float posY = y * -1 + Spacing;

        //spawnedTile.transform.position = new Vector2(posX, posY);

        RectTransform rectTransform = spawnedTile.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(x * Spacing, y * Spacing);
        var tile = spawnedTile.GetComponent<Tile>();
        tile.Setup(x, y, this);

        return tile;
    }
}
