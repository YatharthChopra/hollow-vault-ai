using UnityEngine;

/// <summary>
/// COMBAT state — Sentinel charges and attacks the player at melee range.
///
/// Transitions OUT:
///   → STAGGERED  : Sentinel is hit by the player (TakeDamage called externally)
///   → RALLYING   : HP drops below threshold and Shade is in range
///   → RETURN     : player out of range / lost sight for > memoryDuration seconds
/// </summary>
public class SentinelCombatState : State
{
    private CryptSentinel s;
    private float lostSightTime = -1f; // tracks when player left view cone

    public SentinelCombatState(CryptSentinel sentinel) : base(sentinel)
    {
        s = sentinel;
    }

    public override void Enter()
    {
        s.agent.speed = s.chaseSpeed;
        s.ShowAlertIcon(true);
        lostSightTime = -1f;
        Debug.Log("[Sentinel] Entering COMBAT");
    }

    public override void Execute()
    {
        // Always chase the player
        s.agent.SetDestination(s.player.position);

        bool sees = s.vision.CanSeePlayer(s.player);
        if (sees)
        {
            s.lastKnownPlayerPos = s.player.position;
            lostSightTime = -1f;
        }
        else
        {
            // Start memory countdown when sight is lost
            if (lostSightTime < 0f) lostSightTime = Time.time;

            // After memory expires → give up
            if (Time.time - lostSightTime > s.memoryDuration)
            {
                s.ChangeState(new SentinelReturnState(s));
                return;
            }
        }

        // Check melee range (attack handled via animation event / separate component)
        float distToPlayer = Vector3.Distance(s.transform.position, s.player.position);
        if (distToPlayer < s.combatRange)
        {
            // Melee attack — damage dealt by weapon/animation event in inspector
            // Here we just keep chasing; actual damage is triggered by Attack() below
        }

        // RALLYING CRY — trigger if HP low and not yet rallied
        if (s.currentHP <= s.rallyHPThreshold && !s.hasRallied)
        {
            s.ChangeState(new SentinelRallyingState(s));
        }
    }

    public override void Exit()
    {
        s.ShowAlertIcon(false);
        Debug.Log("[Sentinel] Exiting COMBAT");
    }

    // -------------------------------------------------------
    // Called by an animation event on the Sentinel's attack clip
    // -------------------------------------------------------
    public void Attack()
    {
        float distToPlayer = Vector3.Distance(s.transform.position, s.player.position);
        if (distToPlayer < s.combatRange)
        {
            PlayerHealth playerHealth = s.player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
                playerHealth.TakeDamage(20f);
        }
    }
}
