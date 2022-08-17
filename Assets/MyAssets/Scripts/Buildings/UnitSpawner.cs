using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.EventSystems;
using TMPro;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject unitPrefab = null;
    [SerializeField] private Transform unitSpawnPoint = null;

    #region Server

    [Command]   
    private void CmdSpawnUnit()
    {
        // Spawns on Server
        GameObject prefabInstance = Instantiate(unitPrefab, unitSpawnPoint.position, unitSpawnPoint.rotation);

        // Spawns on Network
        // "connectionToClient" makes sure that the spawned object belongs to me
        NetworkServer.Spawn(prefabInstance, connectionToClient);
    }

    #endregion

    #region Client

    // When the object is clicked
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) { return; }

        if (!hasAuthority) { return; }

        CmdSpawnUnit();
    }    

    #endregion
}
