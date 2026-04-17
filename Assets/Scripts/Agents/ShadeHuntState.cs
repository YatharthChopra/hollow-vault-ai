using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// HUNT — Shade locks onto the player and drains stamina on contact
// Transitions:
//   -> Retreat : torch detected
//   -> Stalk   : player escapes beyond escapeRange
public class ShadeHuntState : State
{
    ShadeBrain shade;

    public ShadeHuntState(ShadeBrain _shade) { shade = _shade; }

    public override void Enter()
    {
        shade.agent.speed = shade.huntSpeed;
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

        shade.agent.SetDestination(shade.playerTransform.position);

        float dist = Vector3.Distance(shade.transform.position, shade.playerTransform.position);

        // drain stamina while in contact range
        if (dist < 1.0f)
        {
            PlayerHealth ph = shade.playerTransform.GetComponent<PlayerHealth>();
            if (ph != null)
                ph.DrainStamina(shade.staminaDrainRate * Time.deltaTime);
        }

        // player got away
        if (dist > shade.escapeRange)
            shade.ChangeState(shade.stalkState);
    }

    public override void Exit() { }

    public override string ToString() => "Hunt";
}
