using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class UnitProjectile : NetworkBehaviour
{
    [SerializeField] private Rigidbody rb = null;
    [SerializeField] private GameObject bulletImpact = null;
    [SerializeField] private float launchForce = 10f;
    [SerializeField] private float destroyAfterSeconds = 5f;
    [SerializeField] private int damage = 5;

    private void Start()
    {
        rb.velocity = transform.forward * launchForce;
    }

    public override void OnStartServer()
    {
        Invoke(nameof(destroySelf), destroyAfterSeconds);
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {

        // If object we hit is owned by us, return
        if (other.TryGetComponent<NetworkIdentity>(out NetworkIdentity networkIdentity))
        {
            if (networkIdentity.connectionToClient == connectionToClient) { return; }
        }

        // If object has a health, deal damage
        if(other.TryGetComponent<Health>(out Health health))
        {
            //GameObject explosion = Instantiate(bulletImpact, transform.position, new Quaternion(0, 0, 1, 0) * transform.rotation);
            //NetworkServer.Spawn(explosion);
            health.dealDamage(damage);
            destroySelf();
        }

        // Object has no health
        //destroySelf();
    }

    [Server]
    private void destroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }
}
