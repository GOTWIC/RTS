using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.EventSystems;
using TMPro;

public class Spawner : NetworkBehaviour//, IPointerClickHandler
{
    [SerializeField] private Transform unitSpawnPoint = null;
    [SerializeField] private GameObject baseUIParent = null;
    [SerializeField] private Transform[] turretLocations = null;
    [SerializeField] private Targetable targetable = null;

    [SerializeField] private GameObject[] spawnableUnits;
    [SerializeField] private GameObject[] spawnableTurrets;

    private GameObject[] turrets = new GameObject[6];
    private List<int> openTurretLocations = new List<int>();

    #region Server

    [Command]
    public void CmdSpawnUnit(int unitID)
    {
        GameObject unitPrefab = spawnableUnits[unitID];

        Vector3 spawnPoint = new Vector3(unitSpawnPoint.position.x, 0, unitSpawnPoint.position.z);

        // Spawns on Server
        GameObject prefabInstance = Instantiate(unitPrefab, spawnPoint, unitSpawnPoint.rotation);

        Debug.Log(spawnPoint);

        // Spawns on Network
        // "connectionToClient" makes sure that the spawned object belongs to me
        NetworkServer.Spawn(prefabInstance, connectionToClient);
    }

    [Command]
    public void CmdSpawnTurret(int turretID)
    {
        if(openTurretLocations.Count == 0) { return; }

        int availableIndex = openTurretLocations[0];

        GameObject turretPrefab = spawnableTurrets[turretID];

        GameObject prefabInstance = Instantiate(turretPrefab, turretLocations[availableIndex].position, turretLocations[availableIndex].rotation);
        NetworkServer.Spawn(prefabInstance, connectionToClient);
        turrets[availableIndex] = prefabInstance;

        // Since the location is now taken, remove it from the list of available spots
        openTurretLocations.RemoveAt(0);

        // Add the turret's target point to the building's targetable script
        targetable.addTargetPoint(prefabInstance.GetComponent<Turret>().getTargetPoint());
    }

    [Server]
    public void ServerSpawnTurret(int turretID)
    {
        ServerTurretUpdate();
        if (openTurretLocations.Count == 0) { return; }

        int availableIndex = openTurretLocations[0];

        GameObject turretPrefab = spawnableTurrets[turretID];

        GameObject prefabInstance = Instantiate(turretPrefab, turretLocations[availableIndex].position, turretLocations[availableIndex].rotation);
        NetworkServer.Spawn(prefabInstance, connectionToClient);
        turrets[availableIndex] = prefabInstance;

        // Since the location is now taken, remove it from the list of available spots
        openTurretLocations.RemoveAt(0);


        // Add the turret's target point to the building's targetable script
        targetable.addTargetPoint(prefabInstance.GetComponent<Turret>().getTargetPoint());
    }

    void Update()
    {
        if (hasAuthority)
        {
            baseUIParent.SetActive(true);
            turretUpdate();
        }
        else
            baseUIParent.SetActive(false);   

    }

    [Command]
    private void turretUpdate()
    {
        // Iterate through each turret
        for (int i = 0; i < turrets.Length; i++)
        {
            GameObject turret = turrets[i];

            // If the index has no turret, then it was destroyed or not created,
            // so add it to the list of available spots
            if (turret == null)
            {
                if (openTurretLocations.Contains(i)) { continue; }
                openTurretLocations.Add(i);
                continue;
            }

            
            // The index has a turret, so update it's position and rotation
            Transform location = (Transform)turretLocations.GetValue(i);
            turret.transform.position = location.position;
            turret.transform.rotation = location.rotation;
        }
    }

    [Server]
    private void ServerTurretUpdate()
    {
        // Iterate through each turret
        for (int i = 0; i < turrets.Length; i++)
        {
            GameObject turret = turrets[i];

            // If the index has no turret, then it was destroyed or not created,
            // so add it to the list of available spots
            if (turret == null)
            {
                if (openTurretLocations.Contains(i)) { continue; }
                openTurretLocations.Add(i);
                continue;
            }


            // The index has a turret, so update it's position and rotation
            Transform location = (Transform)turretLocations.GetValue(i);
            turret.transform.position = location.position;
            turret.transform.rotation = location.rotation;
        }
    }

    // Called by unit to get the nearest turret when targeting a building
    public GameObject[] getTurrets()
    {
        return turrets;
    }
    

    #endregion

    #region Client

    #endregion
}
