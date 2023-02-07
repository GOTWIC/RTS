using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class MyNetworkManager : NetworkManager
{

    [SerializeField] private GameObject unitSpawnerInstance = null;
    [SerializeField] private int numBasesToSpawn = 10;
    [SerializeField] private int maxSpawnRetries = 5;
    [SerializeField] private float minSpawnDistance = 100f;
    [SerializeField] private int[] xSpawnRange = new int[] {-2400, 2400};
    [SerializeField] private int[] zSpawnRange = new int[] { -2400, 2400 };
    private List<GameObject> baseList = new List<GameObject>();

    [SerializeField] private GameObject cube = null;

    private Camera mainCamera = null;



    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);


        // Pick a random base
        int baseNum = Random.Range(0, baseList.Count);

        // Retries
        int maxHomeRetries = 100;
        int retries = 0;

        // Try to randomly spawn first
        while (baseList[baseNum].GetComponent<NetworkIdentity>().connectionToClient != null && retries < maxHomeRetries)
        {
            baseNum = Random.Range(0, baseList.Count);
            retries += 1;
        }

        // If by extreme chance the base is unable to spawn, iterate over the collection to guarentee a spawn
        if(retries == 10)
        {
            for(int i = 0; i < baseList.Count; i++)
            {
                baseNum = Random.Range(0, baseList.Count);
                if (baseList[baseNum].GetComponent<NetworkIdentity>().connectionToClient == null) { break; }
            }
        }

        // Assign base to client
        baseList[baseNum].GetComponent<NetworkIdentity>().AssignClientAuthority(conn);

        // Get the main camera
        mainCamera = Camera.main;

        Vector3 basePos = baseList[baseNum].transform.position;

        // Move the camera to the home base's position
        mainCamera.transform.position = new Vector3(basePos.x, mainCamera.transform.position.y, basePos.z + 100);
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
    }

    public void Update()
    {
        cube.transform.position = new Vector3(cube.transform.position.x + 0.01f, cube.transform.position.y, cube.transform.position.z);
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
                baseList.Add(spawnerInstance);
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

