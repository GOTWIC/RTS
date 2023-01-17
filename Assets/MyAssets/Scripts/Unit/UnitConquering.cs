using Mirror;
using UnityEngine;

public class UnitConquering : NetworkBehaviour
{
    [SerializeField] private Targeter targeter = null;

    [SerializeField] private bool hasValidTarget = false;

    [ServerCallback]
    void Update()
    {
        // Check if there is a target
        Targetable target = targeter.getTarget();

        if(target == null) { return; }

        // Clear and Return if the target isn't a base
        if(target.getTargetType() != "base"){ 
            targeter.clearTarget();
            hasValidTarget = false;
            return;
        }

        // Check if the target is conquerable
        BaseState baseState = target.GetComponent<BaseState>();
        string baseOwnershipState = baseState.getBaseState();

        if(baseOwnershipState != "conquerable"){
            targeter.clearTarget();
            hasValidTarget = false;
            return;
        }

        hasValidTarget = true;

        // If we reach here, then the APC is targetting a base that is conquerable
    }

    [Server]
    public bool HasValidTarget()
    {
        return hasValidTarget;
    }

    [Server]
    public void selfDestruct()
    {
        NetworkServer.Destroy(gameObject);
    }
}
