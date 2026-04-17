using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadeRalliedState : State
{
    ShadeBrain s;

    public ShadeRalliedState(ShadeBrain shade) : base(shade)
    {
        s = shade;
    }

    public override void Enter()
    {
        // summoned by the Sentinel, go straight to stalking
        s.ChangeState(new ShadeStalkState(s));
    }

    public override void Execute()
    {
    }

    public override void Exit()
    {
    }
}
