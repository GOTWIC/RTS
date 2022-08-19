using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Events;
using System;

public class Unit : NetworkBehaviour
{
    [SerializeField] private UnityEvent onSelected = null;
    [SerializeField] private UnityEvent onDeselected = null;
    [SerializeField] private UnitMovement unitMovement = null;
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private Health health = null;
    [SerializeField] private GameObject deathExplosion = null;

    public static event Action<Unit> ServerOnUnitSpawned;
    public static event Action<Unit> ServerOnUnitDespawned;

    public static event Action<Unit> AuthorityOnUnitSpawned;
    public static event Action<Unit> AuthorityOnUnitDespawned;

    private void Awake()
    {
        health.ServerOnDie += dyingSequence;
    }

    private void OnDestroy()
    {
        health.ServerOnDie += dyingSequence;
    }

    public UnitMovement getUnitMovement()
    {
        return unitMovement;
    }

    public Targeter getTargeter()
    {
        return targeter;
    }

    #region Server
        
    public override void OnStartServer()
    {
        ServerOnUnitSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        ServerOnUnitDespawned?.Invoke(this);
    }

    public override void OnStartClient()
    {
        if(!isClientOnly) { return; }

        if(!hasAuthority) { return; }

        AuthorityOnUnitSpawned?.Invoke(this);
    }

    public override void OnStopClient()
    {
        if (!isClientOnly) { return; }

        if (!hasAuthority) { return; }

        AuthorityOnUnitDespawned?.Invoke(this);
    }

    public void dyingSequence()
    {
        GameObject explosion = Instantiate(deathExplosion, transform.position, transform.rotation);
        NetworkServer.Spawn(explosion);

        NetworkServer.Destroy(gameObject);
    }

    #endregion

    #region Client

    [Client]
    public void select()
    {
        if (!hasAuthority) { return; }

        onSelected?.Invoke();
    }

    [Client]
    public void deselect()
    {
        if (!hasAuthority) { return; }

        onDeselected?.Invoke();
    }

    #endregion
}
