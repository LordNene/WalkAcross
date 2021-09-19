using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    static Tooltip instance;

    [SerializeField] RectTransform background;
    [SerializeField] Text tooltipMessage;
    [SerializeField] Camera uiCamera;
    float textPadding = 4f * 2f; // on both sides

    void Start()
    {
        instance = this;
        HideTooltip();
        if (uiCamera == null) GameObject.FindGameObjectWithTag("MainCamera");
    }

    /*void Update()
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), Input.mousePosition, uiCamera, out localPoint);
        transform.localPosition = localPoint;
    }*/

    private void OnValidate()
    {
        gameObject.SetActive(true);
        transform.localPosition = new Vector2(727, 218); // just somewhere outside of screen
    }

    void UpdatePosition()
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), Input.mousePosition, uiCamera, out localPoint);
        transform.localPosition = localPoint;
    }

    void ShowTooltip(string message)
    {
        gameObject.SetActive(true);
        tooltipMessage.text = message;

        var backgroundSize = new Vector2(tooltipMessage.preferredWidth + textPadding, tooltipMessage.preferredHeight + textPadding);
        background.sizeDelta = backgroundSize;
        UpdatePosition();
        //transform.localPosition = new Vector2(position.x, position.y);
    }

    void HideTooltip()
    {
        gameObject.SetActive(false);
    }

    public static void ShowTooltip_Static(string message)
    {
        instance.ShowTooltip(message);
    }

    public static void HideTooltip_Static()
    {
        instance.HideTooltip();
    }
}
