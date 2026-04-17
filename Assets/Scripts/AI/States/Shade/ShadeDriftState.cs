using UnityEngine;

/// <summary>
/// DRIFT state — The Shade floats slowly along the room perimeter.
/// Default idle behaviour.
///
/// Transitions OUT:
///   → ALERT : sound detected within hearingRadius
/// </summary>
public class ShadeDriftState : State
{
    private ShadeBrain s;

    public ShadeDriftState(ShadeBrain shade) : base(shade)
    {
        s = shade;
    }

    public override void Enter()
    {
        s.agent.speed = s.driftSpeed;
        // Begin drifting to first waypoint (perimeter loop)
        if (s.patrolWaypoints.Length > 0)
            s.agent.SetDestination(s.patrolWaypoints[s.waypointIndex].position);
        Debug.Log("[Shade] Entering DRIFT");
    }

    public override void Execute()
    {
        // Advance along perimeter waypoints
        if (s.patrolWaypoints.Length > 0)
        {
            float dist = Vector3.Distance(
                new Vector3(s.transform.position.x, 0f, s.transform.position.z),
                new Vector3(s.patrolWaypoints[s.waypointIndex].position.x, 0f,
                            s.patrolWaypoints[s.waypointIndex].position.z));

            if (dist < s.waypointReachedThreshold)
            {
                s.AdvanceWaypoint();
                s.agent.SetDestination(s.patrolWaypoints[s.waypointIndex].position);
            }
        }

        // Sound detection → ALERT
        if (s.hearing.heardSoundThisFrame)
        {
            s.lastKnownPlayerPos = s.hearing.lastHeardPosition;
            s.ChangeState(new ShadeAlertState(s));
        }
    }

    public override void Exit()
    {
        Debug.Log("[Shade] Exiting DRIFT");
    }
}
