using UnityEngine;

/// <summary>
/// INVESTIGATE state — Sentinel moves to last known noise/sight position.
///
/// Transitions OUT:
///   → COMBAT   : player confirmed in view cone
///   → RETURN   : investigation timed out (no confirmed sighting)
/// </summary>
public class SentinelInvestigateState : State
{
    private CryptSentinel s;

    public SentinelInvestigateState(CryptSentinel sentinel) : base(sentinel)
    {
        s = sentinel;
    }

    public override void Enter()
    {
        s.agent.speed = s.patrolSpeed;
        s.ShowQuestionIcon(true);
        s.agent.SetDestination(s.lastKnownPlayerPos);
        Debug.Log("[Sentinel] Entering INVESTIGATE");
    }

    public override void Execute()
    {
        // If player steps back into the view cone → escalate to COMBAT
        bool sees = s.vision.CanSeePlayer(s.player);
        if (sees)
        {
            s.lastKnownPlayerPos = s.player.position;
            s.ChangeState(new SentinelCombatState(s));
            return;
        }

        // Hearing can refresh the destination (player made more noise nearby)
        if (s.hearing.heardSoundThisFrame)
        {
            s.lastKnownPlayerPos = s.hearing.lastHeardPosition;
            s.agent.SetDestination(s.lastKnownPlayerPos);
        }

        // Timeout → give up and return to patrol
        if (s.TimeInState() > s.investigateTimeout)
        {
            s.ChangeState(new SentinelReturnState(s));
        }
    }

    public override void Exit()
    {
        s.ShowQuestionIcon(false);
        Debug.Log("[Sentinel] Exiting INVESTIGATE");
    }
}
