using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Targeter : NetworkBehaviour
{
    [SerializeField] private Targetable target;
    [SerializeField] private string targetSetter = string.Empty;
    
    public Targetable getTarget()
    {
        return target;
    }

    public string getTargetSetter()
    {
        return targetSetter;
    }

    public void Update()
    {
        // If we don't have a target (ie target was destroyed), then reset the target setter
        if(target == null)
            targetSetter = string.Empty;
    }

    #region Server

    [Command]
    public void CmdSetTarget(GameObject targetObject, string targetSetter)
    {
        if(!targetObject.TryGetComponent<Targetable>(out Targetable target)) { return; }

        this.target = target;
        this.targetSetter = targetSetter;
    }

    [Server]
    public void ServerSetTarget(GameObject targetObject, string targetSetter)
    {
        if (!targetObject.TryGetComponent<Targetable>(out Targetable target)) { return; }

        this.target = target;
        this.targetSetter = targetSetter;
    }

    [Server]
    public void clearTarget()
    {
        target = null;
    }

    #endregion
}
