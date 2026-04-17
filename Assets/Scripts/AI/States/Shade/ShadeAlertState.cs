using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadeAlertState : State
{
    ShadeBrain s;

    public ShadeAlertState(ShadeBrain shade) : base(shade)
    {
        s = shade;
    }

    public override void Enter()
    {
        s.agent.ResetPath();

        // face toward the sound
        Vector3 dir = (s.lastKnownPlayerPos - s.transform.position).normalized;
        dir.y = 0f;
        if (dir != Vector3.zero)
            s.transform.rotation = Quaternion.LookRotation(dir);
    }

    public override void Execute()
    {
        // sound confirmed again, start stalking
        if (s.hearing.heardSoundThisFrame)
        {
            s.lastKnownPlayerPos = s.hearing.lastHeardPosition;
            s.ChangeState(new ShadeStalkState(s));
            return;
        }

        // torch spotted, retreat
        if (s.lightSense.DetectsTorch())
        {
            s.ChangeState(new ShadeRetreatState(s));
            return;
        }

        // sound faded, go back to drifting
        if (s.TimeInState() >= s.alertTimeout)
        {
            s.ChangeState(new ShadeDriftState(s));
        }
    }

    public override void Exit()
    {
    }
}
