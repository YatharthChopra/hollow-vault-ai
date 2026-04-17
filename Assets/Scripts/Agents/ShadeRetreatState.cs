using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// RETREAT — Shade flees from the torch toward the furthest waypoint (shadow)
// Transitions:
//   -> Drift : torch extinguished and reached the shadow waypoint
public class ShadeRetreatState : State
{
    ShadeBrain shade;

    public ShadeRetreatState(ShadeBrain _shade) { shade = _shade; }

    public override void Enter()
    {
        shade.agent.speed = shade.huntSpeed;
        shade.agent.SetDestination(FindShadowWaypoint());
    }

    public override void Execute()
    {
        bool torchOff = !TorchController.IsLit;
        bool arrived = !shade.agent.pathPending && shade.agent.remainingDistance < 0.4f;

        if (torchOff && arrived)
            shade.ChangeState(shade.driftState);
    }

    public override void Exit() { }

    // pick the waypoint furthest from the player to hide
    Vector3 FindShadowWaypoint()
    {
        Vector3 best = shade.transform.position;
        float maxDist = 0f;

        for (int i = 0; i < shade.patrolWaypoints.Length; i++)
        {
            float d = Vector3.Distance(shade.patrolWaypoints[i].position, shade.playerTransform.position);
            if (d > maxDist)
            {
                maxDist = d;
                best = shade.patrolWaypoints[i].position;
            }
        }
        return best;
    }

    public override string ToString() => "Retreat";
}
