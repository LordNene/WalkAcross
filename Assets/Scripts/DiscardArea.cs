using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardArea : MonoBehaviour
{
    public void OnPointerEnter()
    {
        Tooltip.ShowTooltip_Static("Discard pile");
    }

    public void OnPointerExit()
    {
        Tooltip.HideTooltip_Static();
    }
}
