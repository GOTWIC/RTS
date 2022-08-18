using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;

public class EnemyRadar : NetworkBehaviour
{
    [SerializeField] private RadarCollision radarCollision = null;
    [SerializeField] private NavMeshAgent agent = null;
    [SerializeField] Targeter targeter;
    [SerializeField] private List<Targetable> enemiesInRange = new List<Targetable>();

    #region Server

    public override void OnStartServer()
    {
        radarCollision.ServerOnTriggerEnter += checkEnemyEnter;
        radarCollision.ServerOnTriggerExit += checkEnemyExit;
    }

    public override void OnStopServer()
    {
        radarCollision.ServerOnTriggerEnter -= checkEnemyEnter;
        radarCollision.ServerOnTriggerExit -= checkEnemyExit;
    }

    [ServerCallback]
    private void Update()
    {
        // Check if there is a target
        if(targeter.getTarget() != null)
        {
            // Check if radar set the target
            if(targeter.getTargetSetter() != "radar") { return; }

            // Check if target is still in range, don't do anything if it is
            if (enemiesInRange.Contains(targeter.getTarget())) { return; }

            // If we get here, that means the unit has a target, which was set by the radar,
            // but is out of range. So, remove the target

            targeter.clearTarget();

            return; 
        }

        // For now, do nothing 
        if(agent.hasPath) {}

        scanForEnemy(); 
    }

    [Server]
    private void AddEnemy(Targetable target)
    {
        enemiesInRange.Add(target);
    }

    [Server]
    private void RemoveEnemy(Targetable target)
    {
        enemiesInRange.Remove(target);
    }

    [Server]
    private void scanForEnemy()
    {
        if (enemiesInRange.Count == 0) { return; }

        while (enemiesInRange[0] == null)
        {
            enemiesInRange.RemoveAt(0);
            if (enemiesInRange.Count == 0) { return; }
        }

        // For now, just pick the first enemy
        // Later, we can pick the closest enemy, or we can prioritize defenses/buildings of units, etc
        targeter.ServerSetTarget(enemiesInRange[0].gameObject, "radar");
    }

    #endregion

    #region Client


    private void checkEnemyEnter(Collider other)
    {
        //Debug.Log("Entered: " + other.gameObject);

        if (other.TryGetComponent<Targetable>(out Targetable target))
        {
            // Return if the other object is owned by the same person who owns this unit
            if(other.TryGetComponent<NetworkIdentity>(out NetworkIdentity networkIdentity))
            {
                if (networkIdentity.connectionToClient == connectionToClient) { return; }
            }
            
            // For now, don't include buildings
            if(!other.TryGetComponent<Unit>(out Unit unit)) { return; }

            // If we get over here, this means that something entered our detection range which
            // is targeable, not a friendly object, and is not a building    

            AddEnemy(target);
            Debug.Log("Enemy has entered range");
        }
    }

    private void checkEnemyExit(Collider other)
    {
        //Debug.Log("Exited" + other.gameObject);

        if (other.TryGetComponent<Targetable>(out Targetable target))
        {
            if (other.TryGetComponent<NetworkIdentity>(out NetworkIdentity networkIdentity))
            {
                if (networkIdentity.connectionToClient == connectionToClient) { return; }
            }

            // For now, don't include buildings
            if (!other.TryGetComponent<Unit>(out Unit unit)) { return; }

            RemoveEnemy(target);
            Debug.Log("Enemy has left range");
        }
    }

    #endregion
}
