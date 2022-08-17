using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class EnemyRadar : NetworkBehaviour
{
    [SerializeField] private RadarCollision radarCollision = null;
    [SerializeField] Targeter Targeter;
    [SerializeField] private List<Targetable> enemiesInRange = new List<Targetable>();

    public override void OnStartServer()
    {
        radarCollision.ServerOnTriggerEnter += checkEnemyEnter;
        radarCollision.ServerOnTriggerExit += checkEnemyExit;
    }

    [Server]
    private void checkEnemyEnter(Collider other)
    {
        //If we don't own this ship, we don't care about it's enemy radar
        if (!hasAuthority) { return; }

        if (other.TryGetComponent<Targetable>(out Targetable target))
        {
            // Return if we own the object
            if (target.hasAuthority) { return; }

            enemiesInRange.Add(target);
            Debug.Log("Enemy has entered range");
        }
    }

    [Server]
    private void checkEnemyExit(Collider other)
    {
        //If we don't own this ship, we don't care about it's enemy radar
        if (!hasAuthority) { return; }

        if (other.TryGetComponent<Targetable>(out Targetable target))
        {
            // Return if we own the object
            if (target.hasAuthority) { return; }

            enemiesInRange.Remove(target);
            Debug.Log("Enemy has left range");
        }
    }
}
