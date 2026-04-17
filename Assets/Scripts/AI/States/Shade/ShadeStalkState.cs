using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadeStalkState : State
{
    ShadeBrain s;
    float lastSoundTime;

    public ShadeStalkState(ShadeBrain shade) : base(shade)
    {
        s = shade;
    }

    public override void Enter()
    {
        s.agent.speed = s.driftSpeed;
        s.agent.SetDestination(s.lastKnownPlayerPos);
        lastSoundTime = Time.time;
    }

    public override void Execute()
    {
        // torch spotted, run
        if (s.lightSense.DetectsTorch())
        {
            s.ChangeState(new ShadeRetreatState(s));
            return;
        }

        // update destination if new sound heard
        if (s.hearing.heardSoundThisFrame)
        {
            s.lastKnownPlayerPos = s.hearing.lastHeardPosition;
            s.agent.SetDestination(s.lastKnownPlayerPos);
            lastSoundTime = Time.time;
        }

        // close enough to player, switch to hunt
        float distToPlayer = Vector3.Distance(s.transform.position, s.player.position);
        if (distToPlayer <= s.huntRange)
        {
            s.ChangeState(new ShadeHuntState(s));
            return;
        }

        // sound has gone quiet, back to alert
        if (Time.time - lastSoundTime > s.alertTimeout)
        {
            s.ChangeState(new ShadeAlertState(s));
        }
    }

    public override void Exit()
    {
    }
}
