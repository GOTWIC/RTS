using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent agent = null;
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private bool stationary = true;
    [SerializeField] private float chaseRange = 50f;

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
                }

                else if (agent.hasPath)
                {
                    agent.ResetPath();
                }

                return;
            }

            else if(targeter.getTargetSetter() == "radar")
            {
                // We don't want to move if the radar set the target
                if (agent.hasPath)
                {
                    agent.ResetPath();
                }

                return;
            }
        }

        else if(agent.hasPath)
        {
            // Prevent scenario where agent position is set and reset in the same frame,
            // causing the agent to not move at all

            if (agent.remainingDistance > agent.stoppingDistance) { return; }

            agent.ResetPath();
        }

        else
        {
            // Set stztionary to true, Check for enemies to set target
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

}
