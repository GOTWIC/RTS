using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Targetable : NetworkBehaviour
{
    [SerializeField] private Transform targetingPoint = null;
    [SerializeField] private List<Transform> targetingPoints = null;
    [SerializeField] private string targetType;

    public Transform getTargetPoint()
    {
        return targetingPoint;
    }

    public List<Transform> getTargetingPoints()
    {
        return targetingPoints;
    }

    public Transform getTargetPoint(Vector3 position)
    {
        Transform closestTurret = null;
        float minDist = Mathf.Infinity;
        foreach (Transform turret in targetingPoints)
        {
            if(turret == null) { continue; }
            float dist = Vector3.Distance(turret.position, position);
            if (dist < minDist)
            {
                closestTurret = turret;
                minDist = dist;
            }
        }
        return closestTurret;
    }

    public void addTargetPoint(Transform targetPoint)
    {
        targetingPoints.Add(targetPoint);
    }

    public void Start()
    {

    }

    [ServerCallback]
    public void Update()
    {
        if (targetingPoints == null) { return; }

        foreach(Transform turret in targetingPoints)
        {
            if (turret == null)
                targetingPoints.Remove(turret);
        }
    }

    public string getTargetType()
    {
        return targetType;
    }

}
