using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentinelInvestigateState : State
{
    CryptSentinel s;

    public SentinelInvestigateState(CryptSentinel sentinel) : base(sentinel)
    {
        s = sentinel;
    }

    public override void Enter()
    {
        s.agent.speed = s.patrolSpeed;
        s.ShowQuestionIcon(true);
        s.agent.SetDestination(s.lastKnownPlayerPos);
    }

    public override void Execute()
    {
        // if we spot the player go to combat
        bool sees = s.vision.CanSeePlayer(s.player);
        if (sees)
        {
            s.lastKnownPlayerPos = s.player.position;
            s.ChangeState(new SentinelCombatState(s));
            return;
        }

        // if we hear something new update the destination
        if (s.hearing.heardSoundThisFrame)
        {
            s.lastKnownPlayerPos = s.hearing.lastHeardPosition;
            s.agent.SetDestination(s.lastKnownPlayerPos);
        }

        // give up after timeout
        if (s.TimeInState() > s.investigateTimeout)
        {
            s.ChangeState(new SentinelReturnState(s));
        }
    }

    public override void Exit()
    {
        s.ShowQuestionIcon(false);
    }
}
