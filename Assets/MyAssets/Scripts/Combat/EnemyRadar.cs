using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;
using System.Linq;

public class EnemyRadar : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent agent = null;
    [SerializeField] Targeter targeter;
    [SerializeField] GameObject radarBounds = null;
    [SerializeField] private List<Targetable> enemiesInRange = new List<Targetable>();

    private float detectionRange = 0f;

    #region Server

    [ServerCallback]
    private void Start()
    {
        // Make sure the radar scales are set up properly (x,y,z should be the same)
        Vector3 radarScale = radarBounds.transform.localScale;
        if(radarScale.x != radarScale.y && radarScale.y != radarScale.z)
        {
            Debug.Log("Radar Scale has not been set properly.");
        }

        else
        {
            detectionRange = radarScale.x / 2; // Divide by 2 because the scale is the diameter, and we want the radius
        }
        
    }

    [ServerCallback]
    private void Update()
    {
        updateEnemyRadar();

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

    [Server]
    private void updateEnemyRadar()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange);
        List<Targetable> targets = new List<Targetable>();

        foreach (var other in hitColliders)
        {
            // Continue if object is not targetable
            if (!other.TryGetComponent<Targetable>(out Targetable target)) { continue; }

            // Continue if the object is a friendly
            if (other.TryGetComponent<NetworkIdentity>(out NetworkIdentity networkIdentity))
            {
                if (networkIdentity.connectionToClient == connectionToClient) { continue; }
            }

            // For now, only target units
            if (!other.TryGetComponent<Unit>(out Unit unit)) { continue; }

            targets.Add(target);
        }

        enemiesInRange = targets;
    }

    #endregion
}
