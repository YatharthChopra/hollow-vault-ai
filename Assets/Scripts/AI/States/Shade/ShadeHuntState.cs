using UnityEngine;

/// <summary>
/// HUNT state — Shade locks onto the player and closes fast, draining stamina on contact.
///
/// Transitions OUT:
///   → RETREAT  : torch detected
///   → STALK    : player escapes beyond escapeRange
/// </summary>
public class ShadeHuntState : State
{
    private ShadeBrain s;

    public ShadeHuntState(ShadeBrain shade) : base(shade)
    {
        s = shade;
    }

    public override void Enter()
    {
        s.agent.speed = s.huntSpeed;
        Debug.Log("[Shade] Entering HUNT");
    }

    public override void Execute()
    {
        // Torch → retreat immediately
        if (s.lightSense.DetectsTorch())
        {
            s.ChangeState(new ShadeRetreatState(s));
            return;
        }

        // Chase player directly
        s.agent.SetDestination(s.player.position);
        s.lastKnownPlayerPos = s.player.position;

        float distToPlayer = Vector3.Distance(s.transform.position, s.player.position);

        // Stamina drain on contact
        if (distToPlayer < 1.0f)
        {
            PlayerHealth ph = s.player.GetComponent<PlayerHealth>();
            if (ph != null)
                ph.DrainStamina(s.staminaDrainRate * Time.deltaTime);
        }

        // Player escaped
        if (distToPlayer > s.escapeRange)
        {
            s.ChangeState(new ShadeStalkState(s));
        }
    }

    public override void Exit()
    {
        Debug.Log("[Shade] Exiting HUNT");
    }
}
