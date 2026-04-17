using UnityEngine;

/// <summary>
/// RALLIED state — entered via Sentinel's RALLYING CRY (agent-agent interaction).
/// Shade immediately transitions into STALK toward the Sentinel's last known player pos.
///
/// This state exists as a discrete node in the FSM so the transition is explicit
/// and visible in the diagram (per the design document).
///
/// Transitions OUT:
///   → STALK : immediately on Enter (one-frame state)
/// </summary>
public class ShadeRalliedState : State
{
    private ShadeBrain s;

    public ShadeRalliedState(ShadeBrain shade) : base(shade)
    {
        s = shade;
    }

    public override void Enter()
    {
        Debug.Log("[Shade] Entering RALLIED — transitioning to STALK");
        // Immediately move into Stalk toward the Sentinel's reported player position
        s.ChangeState(new ShadeStalkState(s));
    }

    public override void Execute() { /* never reached — state transitions in Enter */ }

    public override void Exit()
    {
        Debug.Log("[Shade] Exiting RALLIED");
    }
}
