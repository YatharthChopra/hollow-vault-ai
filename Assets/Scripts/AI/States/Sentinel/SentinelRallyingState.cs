using UnityEngine;

/// <summary>
/// RALLYING CRY state — Sentinel briefly pauses and summons The Shade.
/// This is the core agent-agent interaction in Hollow Vault.
///
/// Transitions OUT:
///   → COMBAT  : after the cry animation / delay, resumes chasing player
/// </summary>
public class SentinelRallyingState : State
{
    private CryptSentinel s;
    private const float CRY_DURATION = 1.2f; // seconds the rally animation plays

    public SentinelRallyingState(CryptSentinel sentinel) : base(sentinel)
    {
        s = sentinel;
    }

    public override void Enter()
    {
        // Stop to "shout"
        s.agent.ResetPath();
        s.agent.velocity = Vector3.zero;

        // Attempt to alert The Shade
        bool rallied = s.TryRally();
        Debug.Log($"[Sentinel] RALLYING CRY — Shade alerted: {rallied}");
    }

    public override void Execute()
    {
        if (s.TimeInState() >= CRY_DURATION)
        {
            // Resume chasing after the cry
            s.ChangeState(new SentinelCombatState(s));
        }
    }

    public override void Exit()
    {
        Debug.Log("[Sentinel] Exiting RALLYING");
    }
}
