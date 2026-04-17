using UnityEngine;

/// <summary>
/// The Shade — spectral stalker that reacts to sound and light.
///
/// States: DRIFT → ALERT → STALK → HUNT → RETREAT → RALLIED
///
/// Sensors: HearingSensor (radius 8m) + LightSensor (radius 5m).
/// The Shade ignores walls/obstacles when moving (NavMeshAgent still used for
/// ground-plane movement; adjust as needed for flying/intangible feel).
///
/// Agent-agent interaction: receives ReceiveRally() from CryptSentinel.
/// </summary>
public class ShadeBrain : AIBrain
{
    // -------------------------------------------------------------------------
    // Shade-specific config
    // -------------------------------------------------------------------------

    [Header("Shade Config")]
    public float driftSpeed  = 2.5f;
    public float huntSpeed   = 4.5f;
    public float alertTimeout = 3f;   // seconds before ALERT → DRIFT if no confirmation
    public float huntRange   = 3f;    // distance at which STALK becomes HUNT
    public float escapeRange = 5f;    // distance at which HUNT returns to STALK/DRIFT

    [Header("Stamina Drain")]
    [Tooltip("Stamina drained per second while player is in HUNT contact range.")]
    public float staminaDrainRate = 10f;

    // -------------------------------------------------------------------------
    // Sensors
    // -------------------------------------------------------------------------

    [HideInInspector] public HearingSensor hearing;
    [HideInInspector] public LightSensor   lightSense;

    // -------------------------------------------------------------------------
    // Lifecycle
    // -------------------------------------------------------------------------

    protected override void Awake()
    {
        base.Awake();
        hearing    = GetComponent<HearingSensor>();
        lightSense = GetComponent<LightSensor>();

        patrolSpeed = driftSpeed;
        chaseSpeed  = huntSpeed;

        // Start in DRIFT
        InitialState(new ShadeDriftState(this));
    }

    protected override void Update()
    {
        hearing.ResetFrame();
        base.Update();
    }

    // -------------------------------------------------------------------------
    // Public API
    // -------------------------------------------------------------------------

    /// <summary>
    /// Called by CryptSentinel when it broadcasts a rally cry.
    /// Forces the Shade to enter RALLIED (which immediately transitions to STALK).
    /// </summary>
    public void ReceiveRally(Vector3 targetPosition)
    {
        lastKnownPlayerPos = targetPosition;
        ChangeState(new ShadeRalliedState(this));
    }
}
