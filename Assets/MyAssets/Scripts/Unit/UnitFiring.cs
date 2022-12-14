using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;

public class UnitFiring : NetworkBehaviour
{
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private GameObject projectilePrefab = null;
    [SerializeField] private Transform projectileSpawnPoint = null;
    [SerializeField] private NavMeshAgent agent = null;
    [SerializeField] private UnitMovement unitMovement = null;
    [SerializeField] private float firingRange = 60f;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float rotationSpeed = 20f;

    private float lastFireTime = 0f;

    [ServerCallback]
    private void Update()
    {
        Targetable target = targeter.getTarget();

        if(target == null) {
            // If we don't have a target, set the agent's angular speed to normal
            agent.angularSpeed = unitMovement.getAgentRotationSpeed();
            return;
        }

        // Check if we can fire
        if (!canFireAtTarget()) { return; }

        // Disable agent rotation
        agent.angularSpeed = 0;

        // Rotate to target
        Quaternion targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        
        
        if (Time.time > (1 / fireRate) + lastFireTime)
        {
            Quaternion projectileRotation = Quaternion.LookRotation(
                target.getTargetPoint().position - projectileSpawnPoint.position);
            projectileRotation = transform.rotation;
            GameObject projectileInstance = Instantiate(
                projectilePrefab, projectileSpawnPoint.position, projectileRotation);

            NetworkServer.Spawn(projectileInstance, connectionToClient);

            lastFireTime = Time.time;
        }
    }

    [Server]
    private bool canFireAtTarget()
    {
        return ((targeter.getTarget().transform.position - transform.position).sqrMagnitude < firingRange * firingRange);
    }
}
