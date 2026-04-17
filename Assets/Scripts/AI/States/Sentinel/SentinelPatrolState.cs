using UnityEngine;

/// <summary>
/// PATROL state — Crypt Sentinel walks between waypoints in a loop.
///
/// Transitions OUT:
///   → INVESTIGATE  : hears a noise (HearingSensor) OR sees player (VisionSensor)
///   → IDLE (brief pause at each waypoint via the patrol loop)
/// </summary>
public class SentinelPatrolState : State
{
    private CryptSentinel s; // typed reference for convenience

    public SentinelPatrolState(CryptSentinel sentinel) : base(sentinel)
    {
        s = sentinel;
    }

    public override void Enter()
    {
        s.agent.speed = s.patrolSpeed;
        s.ShowAlertIcon(false);
        s.ShowQuestionIcon(false);
        Debug.Log("[Sentinel] Entering PATROL");
    }

    public override void Execute()
    {
        // Move toward current waypoint
        if (s.patrolWaypoints.Length == 0) return;

        Transform target = s.patrolWaypoints[s.waypointIndex];
        s.agent.SetDestination(target.position);

        // Check if waypoint reached
        float dist = Vector3.Distance(
            new Vector3(s.transform.position.x, 0f, s.transform.position.z),
            new Vector3(target.position.x,      0f, target.position.z));

        if (dist < s.waypointReachedThreshold)
        {
            s.AdvanceWaypoint();
            // Small idle pause — state stays PATROL, agent stops momentarily
            // (a dedicated IdleState pause could be added for polish)
        }

        // Check vision — direct sighting triggers INVESTIGATE immediately
        bool sees = s.vision.CanSeePlayer(s.player);
        if (sees)
        {
            s.lastKnownPlayerPos = s.player.position;
            s.ChangeState(new SentinelInvestigateState(s));
            return;
        }

        // Check hearing — sound triggers INVESTIGATE at last heard position
        if (s.hearing.heardSoundThisFrame)
        {
            s.lastKnownPlayerPos = s.hearing.lastHeardPosition;
            s.ChangeState(new SentinelInvestigateState(s));
        }
    }

    public override void Exit()
    {
        Debug.Log("[Sentinel] Exiting PATROL");
    }
}
