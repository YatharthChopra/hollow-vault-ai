using UnityEngine;

/// <summary>
/// RETURN state — Sentinel walks back to its first patrol waypoint after losing the player.
///
/// Transitions OUT:
///   → PATROL   : starting waypoint reached
///   → INVESTIGATE : player or noise detected while returning
/// </summary>
public class SentinelReturnState : State
{
    private CryptSentinel s;

    public SentinelReturnState(CryptSentinel sentinel) : base(sentinel)
    {
        s = sentinel;
    }

    public override void Enter()
    {
        s.agent.speed = s.patrolSpeed;
        // Return to the first waypoint (home position)
        s.waypointIndex = 0;
        if (s.patrolWaypoints.Length > 0)
            s.agent.SetDestination(s.patrolWaypoints[0].position);
        Debug.Log("[Sentinel] Entering RETURN");
    }

    public override void Execute()
    {
        // If player detected again while returning → re-investigate
        if (s.vision.CanSeePlayer(s.player))
        {
            s.lastKnownPlayerPos = s.player.position;
            s.ChangeState(new SentinelInvestigateState(s));
            return;
        }
        if (s.hearing.heardSoundThisFrame)
        {
            s.lastKnownPlayerPos = s.hearing.lastHeardPosition;
            s.ChangeState(new SentinelInvestigateState(s));
            return;
        }

        // Check arrival at home waypoint
        if (s.patrolWaypoints.Length == 0) return;
        float dist = Vector3.Distance(
            new Vector3(s.transform.position.x, 0f, s.transform.position.z),
            new Vector3(s.patrolWaypoints[0].position.x, 0f, s.patrolWaypoints[0].position.z));

        if (dist < s.waypointReachedThreshold)
        {
            s.ChangeState(new SentinelPatrolState(s));
        }
    }

    public override void Exit()
    {
        Debug.Log("[Sentinel] Exiting RETURN");
    }
}
