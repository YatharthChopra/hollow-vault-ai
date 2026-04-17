using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentinelReturnState : State
{
    CryptSentinel s;

    public SentinelReturnState(CryptSentinel sentinel) : base(sentinel)
    {
        s = sentinel;
    }

    public override void Enter()
    {
        s.agent.speed = s.patrolSpeed;
        s.waypointIndex = 0;

        if (s.patrolWaypoints.Length > 0)
            s.agent.SetDestination(s.patrolWaypoints[0].position);
    }

    public override void Execute()
    {
        // if player spotted while returning, go investigate
        if (s.vision.CanSeePlayer(s.player))
        {
            s.lastKnownPlayerPos = s.player.position;
            s.ChangeState(new SentinelInvestigateState(s));
            return;
        }

        if (s.hearing.heardSoundThisFrame)
        {
            s.lastKnownPlayerPos = s.hearing.lastHeardPosition;
            s.ChangeState(new SentinelInvestigateState(s));
            return;
        }

        // check if we made it back home
        if (s.patrolWaypoints.Length == 0) return;

        Vector3 myPos = new Vector3(s.transform.position.x, 0f, s.transform.position.z);
        Vector3 homePos = new Vector3(s.patrolWaypoints[0].position.x, 0f, s.patrolWaypoints[0].position.z);
        float dist = Vector3.Distance(myPos, homePos);

        if (dist < s.waypointReachedThreshold)
        {
            s.ChangeState(new SentinelPatrolState(s));
        }
    }

    public override void Exit()
    {
    }
}
