using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyArea : MonoBehaviour
{
    public void OnPointerEnter()
    {
        Tooltip.ShowTooltip_Static("Enemy area");
    }

    public void OnPointerExit()
    {
        Tooltip.HideTooltip_Static();
    }
}
