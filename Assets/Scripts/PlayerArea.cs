using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArea : MonoBehaviour
{
    public void OnPointerEnter()
    {
        Tooltip.ShowTooltip_Static("Your area");
    }

    public void OnPointerExit()
    {
        Tooltip.HideTooltip_Static();
    }
}
