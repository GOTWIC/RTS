using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BuildingEnemyRadar : NetworkBehaviour
{
    [SerializeField] Spawner spawner = null;
    [SerializeField] GameObject radarBounds = null;
    [SerializeField] private float detectionRange = 0f;
    [SerializeField] private List<Targetable> enemiesInRange = new List<Targetable>();

    private 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    [ServerCallback]
    // Update is called once per frame
    void Update()
    {
        updateEnemyRadar();
        manageTarget();
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

            // Only target units
            if (!other.TryGetComponent<Unit>(out Unit unit)) { continue; }

            targets.Add(target);
        }

        enemiesInRange = targets;
    }

    [Server]
    private void manageTarget()
    { 
        GameObject[] turrets = spawner.getTurrets();
        foreach(GameObject turretObj in turrets)
        {
            // First check if the turret even exists
            if(turretObj == null) { continue; }

            // Get the turret script, this is the base's interface to control the turret
            if (!turretObj.TryGetComponent<Turret>(out Turret turret)) { continue; }

            // Check if there is already a target
            if (turret.getTarget() != null)
            {
                // Check if the current target is still in range
                if (enemiesInRange.Contains(turret.getTarget())) { continue; }

                // Target is not in range anymore, clear target
                else
                    turret.ServerClearTarget();
            }

            // Turret has no target, so try to set it.

            // Check if there are any enemies in range
            if (enemiesInRange.Count == 0) { continue; }


            while (enemiesInRange[0] == null)
            {
                enemiesInRange.RemoveAt(0);
                if (enemiesInRange.Count == 0) { continue; }
            }

            // For now, just pick the first enemy
            // Later, we can pick the closest enemy, or we can prioritize defenses/buildings of units, etc
            turret.ServerSetTarget(enemiesInRange[0].gameObject, "base");


        }
    }


}
