using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentinelStaggeredState : State
{
    CryptSentinel s;

    public SentinelStaggeredState(CryptSentinel sentinel) : base(sentinel)
    {
        s = sentinel;
    }

    public override void Enter()
    {
        // stop moving while stunned
        s.agent.ResetPath();
        s.agent.velocity = Vector3.zero;
    }

    public override void Execute()
    {
        if (s.TimeInState() >= s.staggerDuration)
        {
            if (s.currentHP <= s.rallyHPThreshold && !s.hasRallied)
            {
                s.ChangeState(new SentinelRallyingState(s));
            }
            else
            {
                s.ChangeState(new SentinelCombatState(s));
            }
        }
    }

    public override void Exit()
    {
    }
}
