using UnityEngine;

/// <summary>
/// Hearing sensor: AI can "hear" any SoundEvent broadcast within a configurable radius.
///
/// Other game systems (PlayerController, chests, etc.) emit sound events by calling
/// SoundEventBroadcaster.Broadcast().  This component listens passively and stores
/// the most recent sound it heard for the current frame.
///
/// Attach to the same GameObject as the AI agent.
/// </summary>
public class HearingSensor : MonoBehaviour
{
    [Header("Hearing Settings")]
    [Tooltip("Maximum radius within which a sound can be heard, in metres.")]
    public float hearingRadius = 8f;

    // -------------------------------------------------------------------------
    // Runtime state — reset each frame
    // -------------------------------------------------------------------------

    /// <summary>Set to true for exactly one frame when a new sound is heard.</summary>
    [HideInInspector] public bool heardSoundThisFrame;

    /// <summary>World position of the most recently heard sound.</summary>
    [HideInInspector] public Vector3 lastHeardPosition;

    // -------------------------------------------------------------------------
    // Public API
    // -------------------------------------------------------------------------

    /// <summary>
    /// Called by SoundEventBroadcaster for every sound in the scene.
    /// Returns true if this AI can hear the sound (distance ≤ hearingRadius).
    /// </summary>
    public bool CanHear(Vector3 soundWorldPosition, float soundRadius)
    {
        float dist = Vector3.Distance(transform.position, soundWorldPosition);
        // Effective range = sound's own broadcast radius + agent's hearing radius
        bool heard = dist <= Mathf.Min(soundRadius, hearingRadius);
        if (heard)
        {
            heardSoundThisFrame = true;
            lastHeardPosition   = soundWorldPosition;
        }
        return heard;
    }

    /// <summary>Reset per-frame flag. Called by the agent at the start of Update.</summary>
    public void ResetFrame()
    {
        heardSoundThisFrame = false;
    }

    private void OnEnable()  => SoundEventBroadcaster.Register(this);
    private void OnDisable() => SoundEventBroadcaster.Unregister(this);

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 1f, 0f, 0.2f);
        Gizmos.DrawWireSphere(transform.position, hearingRadius);
    }
}
