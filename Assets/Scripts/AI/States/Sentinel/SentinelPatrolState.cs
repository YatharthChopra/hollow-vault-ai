using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentinelPatrolState : State
{
    CryptSentinel s;

    public SentinelPatrolState(CryptSentinel sentinel) : base(sentinel)
    {
        s = sentinel;
    }

    public override void Enter()
    {
        s.agent.speed = s.patrolSpeed;
        s.ShowAlertIcon(false);
        s.ShowQuestionIcon(false);
    }

    public override void Execute()
    {
        if (s.patrolWaypoints.Length == 0) return;

        Transform target = s.patrolWaypoints[s.waypointIndex];
        s.agent.SetDestination(target.position);

        // check if we reached the waypoint
        Vector3 myPos = new Vector3(s.transform.position.x, 0f, s.transform.position.z);
        Vector3 targetPos = new Vector3(target.position.x, 0f, target.position.z);
        float dist = Vector3.Distance(myPos, targetPos);

        if (dist < s.waypointReachedThreshold)
        {
            s.AdvanceWaypoint();
        }

        // check if player is spotted
        bool sees = s.vision.CanSeePlayer(s.player);
        if (sees)
        {
            s.lastKnownPlayerPos = s.player.position;
            s.ChangeState(new SentinelInvestigateState(s));
            return;
        }

        // check if a sound was heard
        if (s.hearing.heardSoundThisFrame)
        {
            s.lastKnownPlayerPos = s.hearing.lastHeardPosition;
            s.ChangeState(new SentinelInvestigateState(s));
        }
    }

    public override void Exit()
    {
    }
}
