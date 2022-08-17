using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ExplosionSelfDestruct : NetworkBehaviour
{
    [SerializeField] private float timeTillDestroyed = 5f;

    private float initializationTime;

    [ServerCallback]
    void Start()
    {
        initializationTime = Time.timeSinceLevelLoad;
    }

    [ServerCallback]
    void Update()
    {
        float timeSinceInitialization = Time.timeSinceLevelLoad - initializationTime;
        if(timeSinceInitialization > timeTillDestroyed)
            NetworkServer.Destroy(gameObject);
    }
}
