using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// RALLIED — entered when the Sentinel fires its rallying cry
// immediately transitions to Stalk toward the Sentinel's last known player position
public class ShadeRalliedState : State
{
    ShadeBrain shade;

    public ShadeRalliedState(ShadeBrain _shade) { shade = _shade; }

    public override void Enter()
    {
        // go straight to stalking
        shade.ChangeState(shade.stalkState);
    }

    public override void Execute() { }
    public override void Exit() { }

    public override string ToString() => "Rallied";
}
