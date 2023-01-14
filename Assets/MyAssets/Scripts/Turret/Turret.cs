using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class Turret : NetworkBehaviour
{
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private GameObject gunBase = null;
    [SerializeField] private Transform targetingPoint = null;
    [SerializeField] private Health health = null;
    [SerializeField] private GameObject deathExplosion = null;

    public void Start()
    {
        
    }

    public void Update()
    {        
    }

    public Targetable getTarget()
    {
        return targeter.getTarget();
    }

    [Server]
    public void ServerSetTarget(GameObject enemy, string targetSetter)
    {
        targeter.ServerSetTarget(enemy, targetSetter);
    }

    [Server]
    public void ServerClearTarget()
    {
        targeter.clearTarget();
    }

    public Transform getTargetPoint()
    {
        return targetingPoint;
    }

    private void Awake()
    {
        health.ServerOnDie += dyingSequence;
    }

    private void OnDestroy()
    {
        health.ServerOnDie += dyingSequence;
    }


    public void dyingSequence()
    {
        GameObject explosion = Instantiate(deathExplosion, transform.position, transform.rotation);
        NetworkServer.Spawn(explosion);

        NetworkServer.Destroy(gameObject);
    }

}
