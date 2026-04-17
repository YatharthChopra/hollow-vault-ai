using UnityEngine;

/// <summary>
/// STAGGERED state — brief stun after the player lands a hit.
/// Gives the player a short window to flee or attack again.
///
/// Transitions OUT:
///   → RALLYING  : HP below rally threshold (after stagger ends)
///   → COMBAT    : stagger duration elapsed
/// </summary>
public class SentinelStaggeredState : State
{
    private CryptSentinel s;

    public SentinelStaggeredState(CryptSentinel sentinel) : base(sentinel)
    {
        s = sentinel;
    }

    public override void Enter()
    {
        // Stop moving during stagger
        s.agent.ResetPath();
        s.agent.velocity = Vector3.zero;
        Debug.Log("[Sentinel] Entering STAGGERED");
    }

    public override void Execute()
    {
        if (s.TimeInState() >= s.staggerDuration)
        {
            // After recovery, check if rally should trigger
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
        Debug.Log("[Sentinel] Exiting STAGGERED");
    }
}
