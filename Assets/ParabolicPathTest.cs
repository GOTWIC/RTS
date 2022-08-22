using System.Collections;
using System.Collections.Generic;
using UnityEngine;  

public class ParabolicPathTest : MonoBehaviour
{

    [SerializeField] private float t;
    [SerializeField] public float height;
    [SerializeField] public float timeToTarget;

    Vector3 startPosition;

    private float initializationTime;

    private void Start()
    {
        startPosition = transform.position;
        initializationTime = Time.timeSinceLevelLoad;
    }

    // Update is called once per frame
    void Update()
    {
        float timeSinceInitialization = Time.timeSinceLevelLoad - initializationTime;

        t = (timeSinceInitialization) / timeToTarget;// - (int)((Time.time) / timeToTarget);
        Vector3 currentPosition = SampleParabola(startPosition, new Vector3(10, 0, 10), height, t);
        transform.position = currentPosition;
    }

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
}
