using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Top-down player movement controller for Hollow Vault.
///
/// Emits sound events via SoundEventBroadcaster when moving.
/// Stamina (from PlayerHealth) influences noise radius:
/// lower stamina → slower movement → louder footstep interval (detected by Shade).
/// </summary>
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerHealth))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 4f;
    public float gravity   = -9.81f;

    [Header("Sound")]
    [Tooltip("Footstep sound event fired every N seconds while moving.")]
    public float footstepInterval   = 0.4f;
    [Tooltip("Base noise radius of footsteps in metres.")]
    public float footstepBaseRadius = 3f;
    [Tooltip("Extra radius added when stamina is low (below 30%).")]
    public float lowStaminaBonus    = 5f;

    [Header("Input")]
    public InputActionReference moveInput;

    private CharacterController controller;
    private PlayerHealth health;
    private float nextFootstepTime;
    private Vector3 velocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        health     = GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        if (controller.isGrounded && velocity.y < 0f)
            velocity.y = -2f;

        Vector2 input = moveInput.action.ReadValue<Vector2>();
        Vector3 move  = new Vector3(input.x, 0f, input.y) * moveSpeed;

        velocity.y += gravity * Time.deltaTime;
        move.y      = velocity.y;

        controller.Move(move * Time.deltaTime);

        // Face movement direction
        Vector3 hMove = new Vector3(move.x, 0f, move.z);
        if (hMove.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(hMove), 15f * Time.deltaTime);

        // Emit footstep sound events while moving
        if (hMove.sqrMagnitude > 0.01f && Time.time >= nextFootstepTime)
        {
            EmitFootstep();
            // Lower stamina → shorter interval (more frequent, louder)
            float staminaMod = 1f - (1f - health.StaminaRatio) * 0.5f;
            nextFootstepTime = Time.time + footstepInterval * staminaMod;
        }
    }

    private void EmitFootstep()
    {
        float radius = footstepBaseRadius;
        // Bonus noise if stamina is low (slow, heavy steps)
        if (health.StaminaRatio < 0.3f)
            radius += lowStaminaBonus;

        SoundEventBroadcaster.Broadcast(transform.position, radius);
    }
}
