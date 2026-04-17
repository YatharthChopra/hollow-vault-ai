using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    public float maxHP = 100f;
    public float maxStamina = 100f;
    public float staminaRegenRate = 5f;

    [HideInInspector] public float currentHP;
    [HideInInspector] public float currentStamina;

    public UnityEvent onDeath;

    public float StaminaRatio => currentStamina / maxStamina;

    private void Awake()
    {
        currentHP = maxHP;
        currentStamina = maxStamina;
    }

    private void Update()
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
            onDeath?.Invoke();
    }

    public void DrainStamina(float amount)
    {
        currentStamina -= amount;
        if (currentStamina < 0) currentStamina = 0;
    }
}
