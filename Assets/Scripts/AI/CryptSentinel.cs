using UnityEngine;
using UnityEngine.AI;
using TMPro;

/// <summary>
/// Crypt Sentinel — heavy undead guardian of the vault.
///
/// States: PATROL → INVESTIGATE → COMBAT → STAGGERED → RALLYING → RETURN
///
/// Sensor: Vision cone (VisionSensor) + passive hearing via HearingSensor.
/// The Sentinel also listens for a noise threshold from HearingSensor to enter
/// INVESTIGATE, and monitors HP to trigger RALLYING CRY.
///
/// Agent-agent interaction: when RALLYING, it broadcasts a RallyEvent that the
/// Shade's ShadeBrain picks up to enter RALLIED state.
/// </summary>
public class CryptSentinel : AIBrain
{
    // -------------------------------------------------------------------------
    // Sentinel-specific config
    // -------------------------------------------------------------------------

    [Header("Sentinel Config")]
    public float investigateTimeout = 6f;   // seconds before returning to patrol
    public float staggerDuration    = 1.5f; // stun window after being hit
    public float combatRange        = 2.0f; // melee attack range
    public float rallyRadius        = 10f;  // max distance to summon Shade
    public float memoryDuration     = 4f;   // seconds to remember last-seen pos

    [Header("HP")]
    public float maxHP = 100f;
    [HideInInspector] public float currentHP;
    public float rallyHPThreshold = 50f;    // triggers RALLYING CRY below this HP

    [Header("Feedback")]
    [Tooltip("'!' icon shown when Sentinel spots the player.")]
    public GameObject alertIcon;
    [Tooltip("'?' icon shown when Sentinel investigates.")]
    public GameObject questionIcon;

    [Header("Shade Reference")]
    [Tooltip("Drag The Shade GameObject here.")]
    public ShadeBrain shade;

    // -------------------------------------------------------------------------
    // Sensors
    // -------------------------------------------------------------------------

    [HideInInspector] public VisionSensor  vision;
    [HideInInspector] public HearingSensor hearing;

    // -------------------------------------------------------------------------
    // Runtime flags shared with states
    // -------------------------------------------------------------------------

    [HideInInspector] public bool hasRallied = false;   // one-shot rally per life

    // -------------------------------------------------------------------------
    // Lifecycle
    // -------------------------------------------------------------------------

    protected override void Awake()
    {
        base.Awake();
        vision  = GetComponent<VisionSensor>();
        hearing = GetComponent<HearingSensor>();
        currentHP = maxHP;

        // Start in PATROL
        InitialState(new SentinelPatrolState(this));
    }

    protected override void Update()
    {
        hearing.ResetFrame(); // clear per-frame sound flag
        base.Update();
    }

    // -------------------------------------------------------------------------
    // Public API used by states
    // -------------------------------------------------------------------------

    /// <summary>Deals damage to the Sentinel; may trigger STAGGERED or RALLYING.</summary>
    public void TakeDamage(float amount)
    {
        currentHP -= amount;
        currentHP  = Mathf.Max(0, currentHP);

        // Always stagger on hit (interrupt current action)
        ChangeState(new SentinelStaggeredState(this));
    }

    /// <summary>
    /// Broadcasts a rally cry to The Shade if it is within rallyRadius.
    /// Returns true if the Shade was alerted.
    /// </summary>
    public bool TryRally()
    {
        if (shade == null) return false;
        float dist = Vector3.Distance(transform.position, shade.transform.position);
        if (dist <= rallyRadius)
        {
            shade.ReceiveRally(lastKnownPlayerPos);
            hasRallied = true;
            return true;
        }
        return false;
    }

    /// <summary>Show/hide the ! alert icon.</summary>
    public void ShowAlertIcon(bool show)
    {
        if (alertIcon != null) alertIcon.SetActive(show);
    }

    /// <summary>Show/hide the ? investigate icon.</summary>
    public void ShowQuestionIcon(bool show)
    {
        if (questionIcon != null) questionIcon.SetActive(show);
    }
}
