using UnityEngine;

/// <summary>
/// ALERT state — Shade orients toward the sound source, pausing briefly.
///
/// Transitions OUT:
///   → STALK  : location confirmed (sound heard again / still active)
///   → DRIFT  : sound fades (timeout, no new sounds)
/// </summary>
public class ShadeAlertState : State
{
    private ShadeBrain s;

    public ShadeAlertState(ShadeBrain shade) : base(shade)
    {
        s = shade;
    }

    public override void Enter()
    {
        s.agent.ResetPath(); // Pause and orient
        // Face sound source
        Vector3 dir = (s.lastKnownPlayerPos - s.transform.position).normalized;
        if (dir != Vector3.zero)
            s.transform.rotation = Quaternion.LookRotation(new Vector3(dir.x, 0f, dir.z));
        Debug.Log("[Shade] Entering ALERT");
    }

    public override void Execute()
    {
        // If we hear the sound again → confirmed, begin stalking
        if (s.hearing.heardSoundThisFrame)
        {
            s.lastKnownPlayerPos = s.hearing.lastHeardPosition;
            s.ChangeState(new ShadeStalkState(s));
            return;
        }

        // Torch detected → retreat immediately
        if (s.lightSense.DetectsTorch())
        {
            s.ChangeState(new ShadeRetreatState(s));
            return;
        }

        // Timeout — sound faded, resume drifting
        if (s.TimeInState() >= s.alertTimeout)
        {
            s.ChangeState(new ShadeDriftState(s));
        }
    }

    public override void Exit()
    {
        Debug.Log("[Shade] Exiting ALERT");
    }
}
