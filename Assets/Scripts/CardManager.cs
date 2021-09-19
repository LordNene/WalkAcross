using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEditor;
using UnityEngine.UI;

// class for generating card prefab variants
// only available in editor, at runtime PrefabUtility doesn't exist, so it needs to be commented out for build
public class CardManager : MonoBehaviour
{
    /*public GameObject CardClassicBlue;
    public GameObject CardClassicRed;
    public GameObject CardDiagonal;
    public GameObject CardSuper;

    int SizeX { get; set; }
    int SizeY { get; set; }
    string prefabPath = "Assets/Resources/Prefabs/GeneratedCards/";

    public CardManager(GameObject[] cards)
    {
        CardClassicBlue = cards[0]; //GameObject.FindGameObjectWithTag("CardClassicBlue"); // .Find("Card-Classic-Blue");
        CardClassicRed = cards[1]; //GameObject.FindGameObjectWithTag("CardClassicRed"); //.Find("Card-Classic-Red");
        CardDiagonal = cards[2]; //GameObject.FindGameObjectWithTag("CardDiagonal"); //.Find("Card-Diagonal");
        CardSuper = cards[3]; //GameObject.FindGameObjectWithTag("CardSuper"); //.Find("Card-Super");

        SizeX = GameBoard.SizeX;
        SizeY = GameBoard.SizeY;
    }

    public List<GameObject> GenerateCards()
    {
        var cards = new List<GameObject>();

        GenerateSuperCards(ref cards);
        GenerateOtherCards(ref cards);

        return cards;
    }

    void GenerateSuperCards(ref List<GameObject> cards) 
    {
        for (int x = 1; x < SizeX + 1; x++)
        {
            GameObject objSource = (GameObject)PrefabUtility.InstantiatePrefab(CardSuper);
            string localPath = prefabPath + objSource.name + x + ".prefab";
            GameObject obj = PrefabUtility.SaveAsPrefabAssetAndConnect(objSource, localPath, InteractionMode.AutomatedAction); //create

            var texts = obj.GetComponentsInChildren<Text>();
            texts[0].text = x.ToString();
            texts[0].color = GetTextColor(Enums.Color.Super);

            var cardScript = obj.GetComponent<Card>();
            cardScript.Setup(new Vector2Int(x, x), Enums.CardType.Super, Enums.Color.Super);

            cards.Add(obj);
            PrefabUtility.SavePrefabAsset(obj); //save changes
        }
    }

    void GenerateOtherCards(ref List<GameObject> cards) 
    {
        for (int x = 1; x < SizeX + 1; x++)
        {
            for (int y = 1; y < SizeY + 1; y++)
            {
                if (x == y)
                {
                    GenerateDiagonalCard(ref cards, x);
                }
                else
                {
                    if (x == SizeX) continue; // for some reason game design document doesn't have cards like "6/1"
                    GenerateClassicCardPair(ref cards, x, y);
                }
            }
        }
    }

    void GenerateDiagonalCard(ref List<GameObject> cards, int x) 
    {
        GameObject objSource = (GameObject)PrefabUtility.InstantiatePrefab(CardDiagonal);
        string localPath = prefabPath + objSource.name + x + "-" + x + ".prefab";
        GameObject obj = PrefabUtility.SaveAsPrefabAssetAndConnect(objSource, localPath, InteractionMode.AutomatedAction);

        var texts = obj.GetComponentsInChildren<Text>();
        texts[0].text = x.ToString();
        texts[0].color = GetTextColor(Enums.Color.Blue);
        texts[1].text = x.ToString();
        texts[1].color = GetTextColor(Enums.Color.Red);

        var cardScript = obj.GetComponent<Card>();
        cardScript.Setup(new Vector2Int(x, x), Enums.CardType.Diagonal, Enums.Color.Both);

        cards.Add(obj);
        PrefabUtility.SavePrefabAsset(obj);
    }

    void GenerateClassicCardPair(ref List<GameObject> cards, int x, int y)
    {
        GameObject objSourceBlue = (GameObject)PrefabUtility.InstantiatePrefab(CardClassicBlue);
        GameObject objSourceRed = (GameObject)PrefabUtility.InstantiatePrefab(CardClassicRed);

        GenerateClassicCardByColor(ref cards, x, y, objSourceBlue, Enums.Color.Blue);
        GenerateClassicCardByColor(ref cards, x, y, objSourceRed, Enums.Color.Red);
    }

    void GenerateClassicCardByColor(ref List<GameObject> cards, int x, int y, GameObject objSource, Enums.Color color)
    {
        string localPath = prefabPath + objSource.name + x + "-" + y + ".prefab";
        GameObject obj = PrefabUtility.SaveAsPrefabAssetAndConnect(objSource, localPath, InteractionMode.AutomatedAction);

        var texts = obj.GetComponentsInChildren<Text>();
        texts[0].text = x.ToString();
        texts[0].color = GetTextColor(color);

        texts[1].text = y.ToString();
        texts[1].color = GetTextColor(color);

        var cardScript = obj.GetComponent<Card>();
        cardScript.Setup(new Vector2Int(x, y), Enums.CardType.Classic, color);

        cards.Add(obj);
        PrefabUtility.SavePrefabAsset(obj);
    }

    Color32 GetTextColor(Enums.Color color)
    {
        switch(color)
        {
            case Enums.Color.Blue:
                return new Color32(63, 75, 160, 255);
            case Enums.Color.Red:
                return new Color32(160, 61, 63, 255);
            //case Enums.Color.Both: // is handled in GenerateDiagonalCard
            case Enums.Color.Super:
            default:
                return new Color32(90, 43, 86, 255); //purple Super
        }
    }*/
}
