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

    [ServerCallback]
    void Start()
    {
        baseID = gameObject.GetComponent<NetworkIdentity>();

        // Check if the base is owned by a player when its spawned (this means its a player's home base)
        if(baseID.connectionToClient != null) {

            Debug.Log("An owned home base!");

            // Add a turret
            spawner.ServerSpawnTurret(0);
            // Set Base State
            baseOwnershipState = "owned";
        }

        initialized = true;
    }

    [ServerCallback]
    void Update()
    {

        if (!initialized) { return; }
        
        if(baseID == null) { return; }

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
