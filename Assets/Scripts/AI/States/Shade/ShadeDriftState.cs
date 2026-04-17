using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadeDriftState : State
{
    ShadeBrain s;

    public ShadeDriftState(ShadeBrain shade) : base(shade)
    {
        s = shade;
    }

    public override void Enter()
    {
        s.agent.speed = s.driftSpeed;

        if (s.patrolWaypoints.Length > 0)
            s.agent.SetDestination(s.patrolWaypoints[s.waypointIndex].position);
    }

    public override void Execute()
    {
        // move along the perimeter waypoints
        if (s.patrolWaypoints.Length > 0)
        {
            Vector3 myPos = new Vector3(s.transform.position.x, 0f, s.transform.position.z);
            Vector3 targetPos = new Vector3(s.patrolWaypoints[s.waypointIndex].position.x, 0f, s.patrolWaypoints[s.waypointIndex].position.z);
            float dist = Vector3.Distance(myPos, targetPos);

            if (dist < s.waypointReachedThreshold)
            {
                s.AdvanceWaypoint();
                s.agent.SetDestination(s.patrolWaypoints[s.waypointIndex].position);
            }
        }

        // react to sound
        if (s.hearing.heardSoundThisFrame)
        {
            s.lastKnownPlayerPos = s.hearing.lastHeardPosition;
            s.ChangeState(new ShadeAlertState(s));
        }
    }

    public override void Exit()
    {
    }
}
