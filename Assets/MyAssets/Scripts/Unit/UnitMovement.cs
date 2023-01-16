using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using System;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent agent = null;
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private float chaseRange = 50f;
    [SerializeField] private float agentRotationSpeed = 600f;
    [SerializeField] private float agentMovementSpeed = 600f;

    private Camera mainCamera;

    #region Server

    // Called and ran on the server. Callback to prevent warnings that Update can't be called by server
    [ServerCallback]
    private void Update()
    {
        Targetable target = targeter.getTarget();

        if (target != null)
        {
            // We want to move if the user set the target
            if (targeter.getTargetSetter() == "user")
            {                
                if ((target.transform.position - transform.position).sqrMagnitude > chaseRange * chaseRange)
                {
                    agent.SetDestination(target.transform.position);
                    agent.speed = agentMovementSpeed;
                }

                else if (agent.hasPath)
                {
                    agent.ResetPath();
                }

                return;
            }

            // We don't want to move if the radar set the target
            // Need to override this, because it stops manual override
            else if(targeter.getTargetSetter() == "radar")
            {
                if (agent.hasPath)
                {
                    //agent.ResetPath();
                }

                return;
            }
        }

        // This is to stop the units for fighting over each other when they are close enough to the target
        else if(agent.hasPath)
        { 
            if (agent.remainingDistance > agent.stoppingDistance) { return; }

            agent.ResetPath();
        }
    }

    [Command]
    public void CmdMove(Vector3 position)
    {
        // Trying to do a normal move should clear the target   
        targeter.clearTarget(); 

        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas)) { return; }

        agent.SetDestination(hit.position);
    }

    #endregion

    public float getAgentRotationSpeed()
    {
        return agentRotationSpeed;
    }

}
