using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRotation : MonoBehaviour
{

    [SerializeField] float rotationSpeed = 10f;

    private float initializationTime;

    [ServerCallback]
    private void Start()
    {
        initializationTime = Time.timeSinceLevelLoad;
    }

    [ServerCallback]
    void Update()
    {
        float timeSinceInitialization = Time.timeSinceLevelLoad - initializationTime;

        //transform.localEulerAngles = new Vector3(0, ((timeSinceInitialization % 360) * rotationSpeed) % 360, 0); // Quaternion.Euler(0, ((timeSinceInitialization%360) * rotationSpeed)%360, 0);
        transform.RotateAround(transform.parent.position, new Vector3(0, 1, 0), rotationSpeed * Time.deltaTime);
    }
}
