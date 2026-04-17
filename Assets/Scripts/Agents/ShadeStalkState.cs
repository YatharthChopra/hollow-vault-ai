using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// STALK — Shade silently moves toward last known sound position
// Transitions:
//   -> Hunt    : player within huntRange
//   -> Alert   : sound goes quiet
//   -> Retreat : torch detected
public class ShadeStalkState : State
{
    ShadeBrain shade;
    float silenceTimer;
    float silenceTimeout;

    public ShadeStalkState(ShadeBrain _shade) { shade = _shade; }

    public override void Enter()
    {
        shade.agent.speed = shade.driftSpeed;
        shade.agent.SetDestination(shade.lastKnownSoundPos);
        silenceTimer = 0f;
        silenceTimeout = shade.alertTimeout;
        GameEvents.OnSoundEmitted += OnSoundHeard;
    }

    public override void Execute()
    {
        if (TorchController.IsLit)
        {
            float distToPlayer = Vector3.Distance(shade.transform.position, shade.playerTransform.position);
            if (distToPlayer <= shade.lightDetectRadius)
            {
                shade.ChangeState(shade.retreatState);
                return;
            }
        }

        float dist = Vector3.Distance(shade.transform.position, shade.playerTransform.position);
        if (dist <= shade.huntRange)
        {
            shade.ChangeState(shade.huntState);
            return;
        }

        silenceTimer += Time.deltaTime;
        if (silenceTimer >= silenceTimeout)
            shade.ChangeState(shade.alertState);
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
            shade.agent.SetDestination(soundPos);
            silenceTimer = 0f;
        }
    }

    public override string ToString() => "Stalk";
}
