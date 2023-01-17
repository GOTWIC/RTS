using UnityEngine;
using Mirror;

public class BaseState : NetworkBehaviour
{
    [SerializeField] private Targetable targetable = null;
    [SyncVar]
    [SerializeField] private string baseOwnershipState = "";

    [SerializeField] private Spawner spawner = null;

    private NetworkIdentity baseID = null;

    private bool initialized = false;



    /* NOTES
     * 
     * All bases on the map that will ever exist are spawned in when the server starts. 
     * As a result, all bases are (at the beginning) owned by the server. 
     * When a player joins, a random base is picked, and is assigned to the player (by assigning a client connection to it)
     * All of this happens in the Network Manager
     * 
     * Base State simply represents the state of the base IRRESPECTIVE OF OWNER. 
     * This means that it has NO RELATION to who owns the base, whether thats the server, the player, or an enemy
     * In technical terms, a base's connectionToClient status is not related to base's status
     * 
     * All bases start off with a single turret, and they are all in the "owned state"
     * When a base's turrets are all gone, it switches to "conquerable"
     * When an APC enters the base, the base enters the "transition" state.
     * After transitioning (assigning authority to new owner, destroying APC, etc.), the base goes back to the "owned" state
     * 
     */



    [ServerCallback]
    void Start()
    {
        baseID = gameObject.GetComponent<NetworkIdentity>();

        // By default, all bases spawned should have one turret
        spawner.ServerSpawnTurret(0);


        // The following lines of code are deprecated, as bases no long spawn with ownership.
        /*
        // Check if the base is owned by a player when its spawned (this means its a player's home base)
        if (baseID.connectionToClient != null) {
            // Set Base State
            baseOwnershipState = "owned";
        }
        */

        initialized = true;
    }

    [ServerCallback]
    void Update()
    {

        if (!initialized) { return; }

        // If the base has turrets, the base is not conquerable
        if(targetable.getTargetingPoints().Count > 0) {baseOwnershipState = "owned"; return; }

        // If we get here, either the base is conquerable or transitioning

        if(baseOwnershipState != "transitioning"){
            baseOwnershipState = "conquerable";

            // Reset Network Connection 
            baseID.RemoveClientAuthority();
        }

        if(baseOwnershipState == "conquerable"){

            Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, 5);

            for (int i = 0; i < hitColliders.Length; i++)
            {
                if (hitColliders[i].TryGetComponent<UnitConquering>(out UnitConquering uc))
                {
                    transitionOwnership(uc);
                }
            }
        }


        // Might want to trigger some events at some point

    }

    [Server]
    public void transitionOwnership(UnitConquering uc)
    {
        baseOwnershipState = "transitioning";

        // Get the new owner client connection from the APC
        NetworkConnectionToClient newOwnerConnection = uc.gameObject.GetComponent<NetworkIdentity>().connectionToClient;

        // Destroy the APC
        uc.selfDestruct();

        // Assign the base to the new owner
        baseID.AssignClientAuthority(newOwnerConnection);

        // Spawn a single turret
        spawner.ServerSpawnTurret(0);

        baseOwnershipState = "owned";

    }

    [Server]
    public string getBaseState()
    {
        return baseOwnershipState;
    }
}
