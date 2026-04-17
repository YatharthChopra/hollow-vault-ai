using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// DRIFT — Shade floats slowly along the room perimeter
// Transitions:
//   -> Alert : hears a sound
public class ShadeDriftState : State
{
    ShadeBrain shade;

    public ShadeDriftState(ShadeBrain _shade) { shade = _shade; }

    public override void Enter()
    {
        shade.agent.speed = shade.driftSpeed;
        GameEvents.OnSoundEmitted += OnSoundHeard;

        if (shade.patrolWaypoints.Length > 0)
            shade.agent.SetDestination(shade.patrolWaypoints[shade.waypointIndex].position);
    }

    public override void Execute()
    {
        if (shade.patrolWaypoints.Length == 0) return;

        if (!shade.agent.pathPending && shade.agent.remainingDistance < 0.4f)
        {
            shade.AdvanceWaypoint();
            shade.agent.SetDestination(shade.patrolWaypoints[shade.waypointIndex].position);
        }
    }

    public override void Exit()
    {
        GameEvents.OnSoundEmitted -= OnSoundHeard;
    }

    void OnSoundHeard(Vector3 soundPos, float intensity)
    {
        float range = shade.hearingRadius * intensity;
        if (Vector3.Distance(shade.transform.position, soundPos) <= range)
        {
            shade.lastKnownSoundPos = soundPos;
            shade.ChangeState(shade.alertState);
        }
    }

    public override string ToString() => "Drift";
}
