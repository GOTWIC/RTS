using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TurretProjectile : NetworkBehaviour
{
    [SerializeField] private float t;
    [SerializeField] public float height;
    [SerializeField] public float timeToTarget;
    [SerializeField] public int damage;
    [SerializeField] private GameObject contactExplosion = null;
    [SerializeField] private Rigidbody rb = null;

    [SerializeField] private GameObject fireTrail = null;
    [SerializeField] private GameObject smokeTrail = null;

    private Vector3 targetPosition = new Vector3(-999,-999,-999);

    private bool hasExploded = false;

    Vector3 startPosition;

    Vector3 previousPosition;

    private float initializationTime;

    [ClientRpc]
    public void setTargetPosition(Vector3 loc)
    {
        targetPosition = loc;
    }

    public override void OnStartServer()
    {
        Invoke(nameof(ServerStartDestroySequence), 2);
    }


    //[ServerCallback]
    private void Start()
    {
        startPosition = transform.position;
        initializationTime = Time.timeSinceLevelLoad;
    }

    //[ServerCallback]

    // Update is called once per frame
    void Update()
    {
        if (targetPosition == new Vector3(-999, -999, -999)) { return; }

        if (hasExploded) { return; }

        float timeSinceInitialization = Time.timeSinceLevelLoad - initializationTime;

        t = (timeSinceInitialization) / timeToTarget;
        Vector3 currentPosition = SampleParabola(startPosition, targetPosition, height, t);
        transform.position = currentPosition;

        transform.LookAt(previousPosition   );

        previousPosition = currentPosition;

        if (!isServer) { return; }

        if (transform.position.y < 0) { ServerStartDestroySequence(); }

        
    }

    //[Server]
    public Vector3 SampleParabola(Vector3 start, Vector3 end, float height, float t)
    {
        float parabolicT = t * 2 - 1;
        if (Mathf.Abs(start.y - end.y) < 0.1f)
        {
            //start and end are roughly level, pretend they are - simpler solution with less steps
            Vector3 travelDirection = end - start;
            Vector3 result = start + t * travelDirection;
            result.y += (-parabolicT * parabolicT + 1) * height;
            return result;
        }
        else
        {
            //start and end are not level, gets more complicated
            Vector3 travelDirection = end - start;
            Vector3 levelDirecteion = end - new Vector3(start.x, end.y, start.z);
            Vector3 right = Vector3.Cross(travelDirection, levelDirecteion);
            Vector3 up = Vector3.Cross(right, travelDirection);
            if (end.y > start.y) up = -up;
            Vector3 result = start + t * travelDirection;
            result += ((-parabolicT * parabolicT + 1) * height) * up.normalized;
            return result;
        }
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
        if (other.TryGetComponent<Health>(out Health health))
        {
            //GameObject explosion = Instantiate(bulletImpact, transform.position, new Quaternion(0, 0, 1, 0) * transform.rotation);
            //NetworkServer.Spawn(explosion);
            health.dealDamage(damage);
            ServerStartDestroySequence();
            ClientStartDestroySequence();
        }

        // Object has no health
        ServerStartDestroySequence();
        ClientStartDestroySequence();
    }

    [Server]
    private void ServerStartDestroySequence()
    {
        if (!hasExploded)
        {
            GameObject explosion = Instantiate(contactExplosion, transform.position, transform.rotation);
            NetworkServer.Spawn(explosion);
            hasExploded = true;
        }

        Destroy(fireTrail);
        smokeTrail.GetComponent<ParticleSystem>().Stop();

        foreach (Transform child in transform)
        {
            if(child.gameObject.name != "Missle") { continue; }

            child.gameObject.SetActive(false);
        }
        Invoke(nameof(ServerDestroySelf), 3f);
    }

    [Server]
    private void ServerDestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }



    [ClientRpc]
    private void ClientStartDestroySequence()
    {
        if (!hasExploded)
        {
            GameObject explosion = Instantiate(contactExplosion, transform.position, transform.rotation);
            NetworkServer.Spawn(explosion);
            hasExploded = true;
        }

        Destroy(fireTrail);
        smokeTrail.GetComponent<ParticleSystem>().Stop();

        foreach (Transform child in transform)
        {
            if (child.gameObject.name != "Missle") { continue; }

            child.gameObject.SetActive(false);
        }
        Invoke(nameof(ClientDestroySelf), 3f);
    }


    private void ClientDestroySelf()
    {
        Destroy(gameObject);
    }


}
