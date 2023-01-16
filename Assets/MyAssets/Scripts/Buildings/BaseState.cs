using UnityEngine;
using Mirror;

public class BaseState : NetworkBehaviour
{
    [SerializeField] private Targetable targetable = null;
    [SyncVar]
    [SerializeField] private string baseOwnershipState = "";

    void Start()
    {
        
    }

    [ServerCallback]
    void Update()
    {
        // If the base has turrets, the base is not conquerable
        if(targetable.getTargetingPoints().Count > 0) {baseOwnershipState = "owned"; return; }

        // If we get here, either the base is conquerable or transitioning

        if(baseOwnershipState != "transitioning") { baseOwnershipState = "conquerable"; }

        // Check if there is an APC nearby
        

        // Might want to trigger some events at some point

        // We need to reset network connection etc
        
    }

    [Server]
    public string getBaseState()
    {
        return baseOwnershipState;
    }
}
