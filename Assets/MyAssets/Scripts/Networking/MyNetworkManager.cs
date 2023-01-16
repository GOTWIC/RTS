using UnityEngine;
using Mirror;

public class MyNetworkManager : NetworkManager
{

    [SerializeField] private GameObject unitSpawnerInstance = null;
    [SerializeField] private int numBasesToSpawn = 10;
    [SerializeField] private int maxSpawnRetries = 5;
    [SerializeField] private float minSpawnDistance = 100f;
    [SerializeField] private int[] xSpawnRange = new int[] {-2400, 2400};
    [SerializeField] private int[] zSpawnRange = new int[] { -2400, 2400 };
    [SerializeField] private LayerMask layerMask = new LayerMask();

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        // Spawns on Server
        // "conn.identity.transform is how you get transform of the player object"
        // conn.identity.transform.position is essentially the spawn points in the game. Might have to change this later for randomization
        GameObject spawnerInstance = Instantiate(unitSpawnerInstance, conn.identity.transform.position, conn.identity.transform.rotation);

        //Spawns on Network
        NetworkServer.Spawn(spawnerInstance, conn);
        
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
    }

    [Server]
    private void LoadBases()
    {
        int basesSpawned = 0;

        for(int i = 0; i < numBasesToSpawn; i++)
        {
            int retries = 0;

            Vector3 spawnPos = new Vector3(-1,-1,-1);

            while (retries < maxSpawnRetries)
            {
                Vector3 randPos = new Vector3(Random.Range(xSpawnRange[0], xSpawnRange[1]), 1, Random.Range(zSpawnRange[0], zSpawnRange[1]));
                Collider[] hitColliders = Physics.OverlapSphere(randPos, minSpawnDistance);

                // Normally we would use layer mask to disregard the floor but it does not seem to work. 
                // As a work around, we check if there is 1 or less colliders. 
                // Normally, this value will always be atleast one because the floor will be counted

                //Debug.Log(hitColliders.Length);

                if (hitColliders.Length > 1){
                    retries += 1;
                    continue;
                }
                else{
                    spawnPos = randPos;
                    break;
                }
            }

            if(spawnPos != new Vector3(-1, -1, -1)){
                GameObject spawnerInstance = Instantiate(unitSpawnerInstance, spawnPos, Quaternion.identity);
                NetworkServer.Spawn(spawnerInstance);
                basesSpawned += 1;
            }
        }

        Debug.Log(basesSpawned);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);

        // NOTE: This string value has to change if the play scene name ever changes
        if (sceneName != "Assets/SCENES/MainScene.unity") { return; }
        LoadBases();
    }




}

