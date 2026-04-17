using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Tracks player health (HP) and stamina.
///
/// HP:      reduced by Sentinel melee attacks.
/// Stamina: drained by Shade contact.  Lower stamina = louder footsteps
///          (the SoundEventBroadcaster reads staminaRatio to scale noise radius).
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHP = 100f;
    [HideInInspector] public float currentHP;

    [Header("Stamina")]
    public float maxStamina = 100f;
    [HideInInspector] public float currentStamina;
    public float staminaRegenRate = 5f; // per second when not being drained

    [Header("Events")]
    public UnityEvent onDeath;
    public UnityEvent onStaminaExhausted;

    // Ratio 0–1; lower = louder footsteps (used by SoundEventBroadcaster)
    public float StaminaRatio => currentStamina / maxStamina;

    private void Awake()
    {
        currentHP      = maxHP;
        currentStamina = maxStamina;
    }

    private void Update()
    {
        // Passive stamina regen
        if (currentStamina < maxStamina)
        {
            currentStamina = Mathf.Min(maxStamina, currentStamina + staminaRegenRate * Time.deltaTime);
        }
    }

    /// <summary>Apply damage from Sentinel melee.</summary>
    public void TakeDamage(float amount)
    {
        currentHP -= amount;
        currentHP  = Mathf.Max(0f, currentHP);
        Debug.Log($"[Player] HP: {currentHP}/{maxHP}");

        if (currentHP <= 0f)
            onDeath?.Invoke();
    }

    /// <summary>Drain stamina — called by Shade in HUNT state each frame.</summary>
    public void DrainStamina(float amount)
    {
        currentStamina -= amount;
        currentStamina  = Mathf.Max(0f, currentStamina);

        if (currentStamina <= 0f)
            onStaminaExhausted?.Invoke();
    }
}
