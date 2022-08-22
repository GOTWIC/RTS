using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class Turret : NetworkBehaviour
{
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private GameObject gunBase = null;

    public void Start()
    {
        
    }

    public void Update()
    {        
    }

    public Targetable getTarget()
    {
        return targeter.getTarget();
    }

    [Server]
    public void ServerSetTarget(GameObject enemy, string targetSetter)
    {
        targeter.ServerSetTarget(enemy, targetSetter);
    }

    [Server]
    public void ServerClearTarget()
    {
        targeter.clearTarget();
    }

}
