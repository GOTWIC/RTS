using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeDisplay : MonoBehaviour
{
    [SerializeField] GameObject radarBoundsParent;
    private void OnMouseEnter()
    {
        radarBoundsParent.SetActive(true);
    }

    private void OnMouseExit()
    {
        radarBoundsParent.SetActive(false);
    }
}
