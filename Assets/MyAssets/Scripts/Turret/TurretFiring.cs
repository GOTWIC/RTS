using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class TurretFiring : NetworkBehaviour
{
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private GameObject gunBase = null;
    [SerializeField] private GameObject vertialRotationModule = null;
    [SerializeField] private GameObject idleReference = null;
    [SerializeField] private GameObject projectilePrefab = null;
    [SerializeField] private Transform projectileSpawnPoint = null;
    [SerializeField] private Animation anim;
    [SerializeField] private float rotationSpeedH = 100f;
    [SerializeField] private float rotationSpeedV = 20f;
    [SerializeField] private float fireRate = 1f;

    private bool isIdle = false;
    private float lastFireTime = 0f;


    [ServerCallback]
    private void Start()
    {
        vertialRotationModule.transform.localEulerAngles = new Vector3 (359, 0, 0);

    }

    [ServerCallback]
    void Update()
    {
        Targetable target = targeter.getTarget();

        if(target != null)
        {
            // Turn off the idle Animation
            isIdle = false;
            anim.Stop();

            Quaternion targetRotationH = Quaternion.LookRotation(target.transform.position - gunBase.transform.position);

            float horizontalRotationDifference = (targetRotationH.eulerAngles - gunBase.transform.rotation.eulerAngles).y;

            // If both horizontal rotation and vertical rotation are the correct values,
            // then try to fire
            if (vertialRotationModule.transform.localRotation.eulerAngles.x % 360 == 320 && horizontalRotationDifference < 1)
            {
                tryToFire();
            }


            // Horizontal Rotation
            targetRotationH = Quaternion.Euler(-90, targetRotationH.eulerAngles.y, targetRotationH.eulerAngles.z);
            gunBase.transform.rotation = Quaternion.RotateTowards(gunBase.transform.rotation, targetRotationH, rotationSpeedH * Time.deltaTime);
            //Debug.Log(targetRotationH.eulerAngles - gunBase.transform.rotation.eulerAngles);


            // Vertical Rotation
            float verticalRotation = vertialRotationModule.transform.localRotation.eulerAngles.x % 360;
            if (verticalRotation > 320 || verticalRotation == 0)
            {
                vertialRotationModule.transform.Rotate(Vector3.left * (rotationSpeedV * Time.deltaTime));
            }

            verticalRotation = vertialRotationModule.transform.localRotation.eulerAngles.x % 360;
            if(verticalRotation < 320)
                vertialRotationModule.transform.localEulerAngles = new Vector3(320, 0, 0);


        }

        else
        {
            // If the target is null, first we want to check if idle animation is playing
            // If it is, we don't want to disturb it
            if (isIdle) { return; }

            // Turret is not idle but is not firing. Return to idle animation

            // Make sure both vertical and horizontal rotations are reset before starting idle animation
            if (gunBase.transform.localEulerAngles == new Vector3(270, 0, 0) && vertialRotationModule.transform.localEulerAngles.x == 359)
            {
                float idleDelay = UnityEngine.Random.Range(2f, 5f);
                Invoke(nameof(setIdleTrue), idleDelay);
            }

            // Reset horizontal rotation
            Quaternion targetRotationH = Quaternion.LookRotation(idleReference.transform.position - gunBase.transform.position);
            targetRotationH *= Quaternion.Euler(Vector3.right * -90);
            gunBase.transform.rotation = Quaternion.RotateTowards(gunBase.transform.rotation, targetRotationH, rotationSpeedH * Time.deltaTime);



            // Vertical Rotation
            float verticalRotation = vertialRotationModule.transform.localRotation.eulerAngles.x % 360;
            if(verticalRotation > 359) { vertialRotationModule.transform.localEulerAngles = new Vector3(359, 0, 0); return; }
            if (verticalRotation < 359)
            {
                vertialRotationModule.transform.Rotate(Vector3.right * (rotationSpeedV * Time.deltaTime));
            }
        }
    }

    private void setIdleTrue()
    {
        isIdle = true;
        anim.Play();
    }

    private void tryToFire()
    {
        if (Time.time > (1 / fireRate) + lastFireTime)
        {
            //Quaternion projectileRotation = projectileSpawnPoint.transform.rotation;
            GameObject projectileInstance = Instantiate(
                projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
            TurretProjectile projectile = projectileInstance.GetComponent<TurretProjectile>();
            NetworkServer.Spawn(projectileInstance, connectionToClient);

            //projectile.setHeight(20f);
            //projectile.setTimeToTarget(4f);
            //projectile.setSpawnPosition(projectileSpawnPoint.position);
            projectile.setTargetPosition(targeter.getTarget().getTargetPoint().position);

            //Debug.Log(projectileSpawnPoint.position);
            //Debug.Log(targeter.getTarget().getTargetPoint().position);

            lastFireTime = Time.time;
        }
    }

}
