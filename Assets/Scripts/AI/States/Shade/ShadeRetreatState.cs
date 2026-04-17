using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadeRetreatState : State
{
    ShadeBrain s;

    public ShadeRetreatState(ShadeBrain shade) : base(shade)
    {
        s = shade;
    }

    public override void Enter()
    {
        s.agent.speed = s.huntSpeed;
        s.agent.SetDestination(FindShadowWaypoint());
    }

    public override void Execute()
    {
        bool torchGone = !s.lightSense.DetectsTorch();
        float dist = s.agent.remainingDistance;
        bool arrived = dist < s.waypointReachedThreshold && !s.agent.pathPending;

        // only resume once torch is out and we reached the shadow
        if (torchGone && arrived)
        {
            s.ChangeState(new ShadeDriftState(s));
        }
    }

    public override void Exit()
    {
    }

    // find the waypoint furthest from the player to hide in shadow
    Vector3 FindShadowWaypoint()
    {
        Vector3 best = s.transform.position;
        float maxDist = 0f;

        for (int i = 0; i < s.patrolWaypoints.Length; i++)
        {
            float d = Vector3.Distance(s.patrolWaypoints[i].position, s.player.position);
            if (d > maxDist)
            {
                maxDist = d;
                best = s.patrolWaypoints[i].position;
            }
        }
        return best;
    }
}
