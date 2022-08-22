using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Targetable : NetworkBehaviour
{
    [SerializeField] private Transform targetingPoint = null;

    public Transform getTargetPoint()
    {
        return targetingPoint;
    }

    public void Start()
    {
        if (targetingPoint == null)
            Debug.Log("Targeting Point has not been set!");
    }
}
