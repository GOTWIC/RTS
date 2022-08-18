using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RadarCollision : MonoBehaviour
{
    public event Action<Collider> ServerOnTriggerEnter;
    public event Action<Collider> ServerOnTriggerExit;

    private void OnTriggerEnter(Collider other)
    {
        ServerOnTriggerEnter?.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        ServerOnTriggerExit?.Invoke(other);
    }
}
