using UnityEngine;

/// <summary>
/// STALK state — Shade silently moves toward the last known sound origin.
///
/// Transitions OUT:
///   → HUNT     : player within huntRange
///   → ALERT    : sound location lost (no new sounds for alertTimeout seconds)
///   → RETREAT  : torch detected
/// </summary>
public class ShadeStalkState : State
{
    private ShadeBrain s;
    private float lastSoundTime;

    public ShadeStalkState(ShadeBrain shade) : base(shade)
    {
        s = shade;
    }

    public override void Enter()
    {
        s.agent.speed = s.driftSpeed; // Stalk at drift speed — silent approach
        s.agent.SetDestination(s.lastKnownPlayerPos);
        lastSoundTime = Time.time;
        Debug.Log("[Shade] Entering STALK");
    }

    public override void Execute()
    {
        // Torch → retreat
        if (s.lightSense.DetectsTorch())
        {
            s.ChangeState(new ShadeRetreatState(s));
            return;
        }

        // Refresh destination on new sound
        if (s.hearing.heardSoundThisFrame)
        {
            s.lastKnownPlayerPos = s.hearing.lastHeardPosition;
            s.agent.SetDestination(s.lastKnownPlayerPos);
            lastSoundTime = Time.time;
        }

        // Close enough → HUNT
        float distToPlayer = Vector3.Distance(s.transform.position, s.player.position);
        if (distToPlayer <= s.huntRange)
        {
            s.ChangeState(new ShadeHuntState(s));
            return;
        }

        // Sound faded — go back to alert
        if (Time.time - lastSoundTime > s.alertTimeout)
        {
            s.ChangeState(new ShadeAlertState(s));
        }
    }

    public override void Exit()
    {
        Debug.Log("[Shade] Exiting STALK");
    }
}
