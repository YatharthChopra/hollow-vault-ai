using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHP = 100f;
    public float maxStamina = 100f;
    public float staminaRegenRate = 5f;

    [HideInInspector] public float currentHP;
    [HideInInspector] public float currentStamina;

    // 0-1 ratio used by PlayerController to scale noise
    public float StaminaRatio => currentStamina / maxStamina;

    void Awake()
    {
        currentHP = maxHP;
        currentStamina = maxStamina;
    }

    void Update()
    {
        // regen stamina over time
        if (currentStamina < maxStamina)
            currentStamina = Mathf.Min(maxStamina, currentStamina + staminaRegenRate * Time.deltaTime);
    }

    public void TakeDamage(float amount)
    {
        currentHP -= amount;
        if (currentHP < 0) currentHP = 0;

        if (currentHP <= 0)
            GameEvents.OnPlayerDied?.Invoke();
    }

    public void DrainStamina(float amount)
    {
        currentStamina -= amount;
        if (currentStamina < 0) currentStamina = 0;
    }
}
