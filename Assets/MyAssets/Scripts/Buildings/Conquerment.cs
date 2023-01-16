using UnityEngine;
using Mirror;

public class Conquerment : NetworkBehaviour
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




        
    }
}
