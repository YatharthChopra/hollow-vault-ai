using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentinelCombatState : State
{
    CryptSentinel s;
    float lostSightTime = -1f;

    public SentinelCombatState(CryptSentinel sentinel) : base(sentinel)
    {
        s = sentinel;
    }

    public override void Enter()
    {
        s.agent.speed = s.chaseSpeed;
        s.ShowAlertIcon(true);
        lostSightTime = -1f;
    }

    public override void Execute()
    {
        s.agent.SetDestination(s.player.position);

        bool sees = s.vision.CanSeePlayer(s.player);
        if (sees)
        {
            s.lastKnownPlayerPos = s.player.position;
            lostSightTime = -1f;
        }
        else
        {
            if (lostSightTime < 0f) lostSightTime = Time.time;

            // lost sight for too long, go back to patrol
            if (Time.time - lostSightTime > s.memoryDuration)
            {
                s.ChangeState(new SentinelReturnState(s));
                return;
            }
        }

        // trigger rallying cry if health is low
        if (s.currentHP <= s.rallyHPThreshold && !s.hasRallied)
        {
            s.ChangeState(new SentinelRallyingState(s));
        }
    }

    public override void Exit()
    {
        s.ShowAlertIcon(false);
    }
}
