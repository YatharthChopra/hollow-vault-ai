using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentinelRallyingState : State
{
    CryptSentinel s;

    // how long the rally animation plays before resuming
    float cryDuration = 1.2f;

    public SentinelRallyingState(CryptSentinel sentinel) : base(sentinel)
    {
        s = sentinel;
    }

    public override void Enter()
    {
        s.agent.ResetPath();
        s.agent.velocity = Vector3.zero;

        // call the shade
        s.TryRally();
    }

    public override void Execute()
    {
        if (s.TimeInState() >= cryDuration)
        {
            s.ChangeState(new SentinelCombatState(s));
        }
    }

    public override void Exit()
    {
    }
}
