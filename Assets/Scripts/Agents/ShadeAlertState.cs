using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ALERT — Shade heard something, orients toward the source and waits for confirmation
// Transitions:
//   -> Stalk   : hears sound again (confirmed)
//   -> Drift   : sound fades (timeout)
//   -> Retreat : torch detected
public class ShadeAlertState : State
{
    ShadeBrain shade;
    float timer;
    bool confirmed;

    public ShadeAlertState(ShadeBrain _shade) { shade = _shade; }

    public override void Enter()
    {
        shade.agent.ResetPath();
        timer = shade.alertTimeout;
        confirmed = false;
        GameEvents.OnSoundEmitted += OnSoundHeard;

        // face toward the sound
        Vector3 dir = (shade.lastKnownSoundPos - shade.transform.position).normalized;
        dir.y = 0f;
        if (dir != Vector3.zero)
            shade.transform.rotation = Quaternion.LookRotation(dir);
    }

    public override void Execute()
    {
        if (TorchController.IsLit)
        {
            float dist = Vector3.Distance(shade.transform.position, shade.playerTransform.position);
            if (dist <= shade.lightDetectRadius)
            {
                shade.ChangeState(shade.retreatState);
                return;
            }
        }

        if (!confirmed)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
                shade.ChangeState(shade.driftState);
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
            confirmed = true;
            shade.ChangeState(shade.stalkState);
        }
    }

    public override string ToString() => "Alert";
}
