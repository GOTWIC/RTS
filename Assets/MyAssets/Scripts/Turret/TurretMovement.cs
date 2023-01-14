using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using UnityEngine.Rendering;

public class TurretMovement : NetworkBehaviour
{
    [SerializeField] private Targeter targeter = null;

    [SerializeField] private Transform horizontalModule = null;
    [SerializeField] private Transform verticalModule = null;
    [SerializeField] private Transform idleReference = null;
    [SerializeField] private Animation idleAnim;


    [SerializeField] private GameObject projectilePrefab = null;
    [SerializeField] private GameObject smokePrefab = null;
    [SerializeField] private Transform projectileSpawnPoint = null;


    [SerializeField] private float rotationSpeedH = 100f;
    [SerializeField] private float rotationSpeedV = 20f;
    [SerializeField] private float fireRate = 1f;

    private bool isIdle = false;
    private float lastFireTime = 0f;


    [ServerCallback]
    private void Start()
    {
        verticalModule.localEulerAngles = new Vector3 (359, 0, 0);
    }

    [ServerCallback]
    void Update()
    {
        Targetable target = targeter.getTarget();

        if (target != null)
        {
            // Turn off the idle Animation
            StopIdleAnimation();

            // Rotate Horizontally
            int hRotDiff = RotateHorizontally(target.transform);

            // Rotate Vertically
            int vRotDiff = RotateVertically(45);

            // Try to fire
            tryToFire(hRotDiff, vRotDiff);
        }

        else
        {
            // If idle animation is playing, return
            if (isIdle) { return; }

            // Horizontal Rotation
            int hRotDiff = RotateHorizontally(idleReference);

            // Vertical Rotation
            int vRotDiff = RotateVertically(1);

            // Try to start idle animation
            tryToIdle(hRotDiff, vRotDiff);
        }
    }

    private int RotateHorizontally(Transform target)
    {
        // Get the rotation we need
        Quaternion targetRotationH = Quaternion.LookRotation(target.position - horizontalModule.position);
        // Get the current rotation
        Vector3 currentRot = horizontalModule.rotation.eulerAngles;
        // Lock the x-axis rotation (vertical rotation) and z-axis rotation (sideways rotation)
        targetRotationH = Quaternion.Euler(currentRot.x, targetRotationH.eulerAngles.y, currentRot.z);
        // Rotate the horizontal module
        horizontalModule.rotation = Quaternion.RotateTowards(horizontalModule.rotation, targetRotationH, rotationSpeedH * Time.deltaTime);
        // Return the distance left to rotate
        return (int)(targetRotationH.eulerAngles - horizontalModule.rotation.eulerAngles).y;
    }

    private int RotateVertically(int angle)
    {
        Quaternion targetRotationV = verticalModule.rotation;
        targetRotationV = Quaternion.Euler(270 + angle, verticalModule.rotation.eulerAngles.y, verticalModule.rotation.eulerAngles.z);
        verticalModule.rotation = Quaternion.RotateTowards(verticalModule.rotation, targetRotationV, rotationSpeedV * Time.deltaTime);
        return (int)(targetRotationV.eulerAngles - verticalModule.rotation.eulerAngles).x;
    }

    private void tryToFire(int hDiff, int vDiff)
    {
        // Return if it isn't time to fire yet
        if (Time.time <= (1 / fireRate) + lastFireTime) { return; }

        // Return if the turret is not in position yet
        if (hDiff > 5 || vDiff > 5) { return; }

        // Turret is ready to fire and is in position
        GameObject projectileInstance = Instantiate(
                projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
        TurretProjectile projectile = projectileInstance.GetComponent<TurretProjectile>();
        NetworkServer.Spawn(projectileInstance, connectionToClient);

        /*
        GameObject smokeInstance = Instantiate(
                smokePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
        TurretProjectile smoke = smokeInstance.GetComponent<TurretProjectile>();
        NetworkServer.Spawn(smokeInstance, connectionToClient);
        */

        projectile.setTargetPosition(targeter.getTarget().getTargetPoint().position);

        lastFireTime = Time.time;
    }

    private void tryToIdle(int hDiff, int vDiff)
    {
        // Return if vertical and horizontal rotation have not reset
        if (hDiff != 0 || vDiff != 0) { return; }
        isIdle = true;
        float idleDelay = UnityEngine.Random.Range(1f, 5f);
        Invoke(nameof(StartIdleAnimation), idleDelay);
    }

    private void StopIdleAnimation()
    {
        isIdle = false;
        idleAnim.Stop();
    }

    private void StartIdleAnimation()
    {
        // Because we start the idle animation at a delay, we want to return 
        // if a target appeared between "isIdle" being set to true and
        // this function being called
        if (!isIdle) { return; }
        idleAnim.Play();
    }

}
