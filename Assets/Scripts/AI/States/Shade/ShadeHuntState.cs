using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadeHuntState : State
{
    ShadeBrain s;

    public ShadeHuntState(ShadeBrain shade) : base(shade)
    {
        s = shade;
    }

    public override void Enter()
    {
        s.agent.speed = s.huntSpeed;
    }

    public override void Execute()
    {
        // torch spotted, retreat
        if (s.lightSense.DetectsTorch())
        {
            s.ChangeState(new ShadeRetreatState(s));
            return;
        }

        // chase the player
        s.agent.SetDestination(s.player.position);
        s.lastKnownPlayerPos = s.player.position;

        float distToPlayer = Vector3.Distance(s.transform.position, s.player.position);

        // drain stamina on contact
        if (distToPlayer < 1.0f)
        {
            PlayerHealth ph = s.player.GetComponent<PlayerHealth>();
            if (ph != null)
                ph.DrainStamina(s.staminaDrainRate * Time.deltaTime);
        }

        // player escaped
        if (distToPlayer > s.escapeRange)
        {
            s.ChangeState(new ShadeStalkState(s));
        }
    }

    public override void Exit()
    {
    }
}
