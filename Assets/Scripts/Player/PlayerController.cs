using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 4f;
    public float gravity = -9.81f;
    public float footstepInterval = 0.4f;
    public float footstepBaseRadius = 3f;
    public float lowStaminaBonus = 5f;

    public InputActionReference moveInput;

    CharacterController controller;
    PlayerHealth health;
    float nextFootstepTime;
    Vector3 velocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        health = GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        Move();
    }

    void Move()
    {
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        Vector2 input = moveInput.action.ReadValue<Vector2>();
        Vector3 move = new Vector3(input.x, 0f, input.y) * moveSpeed;

        velocity.y += gravity * Time.deltaTime;
        move.y = velocity.y;

        controller.Move(move * Time.deltaTime);

        // face the direction of movement
        Vector3 hMove = new Vector3(move.x, 0f, move.z);
        if (hMove.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(hMove);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 15f * Time.deltaTime);
        }

        // emit footstep sounds while moving
        if (hMove.sqrMagnitude > 0.01f && Time.time >= nextFootstepTime)
        {
            EmitFootstep();
            float staminaMod = 1f - (1f - health.StaminaRatio) * 0.5f;
            nextFootstepTime = Time.time + footstepInterval * staminaMod;
        }
    }

    void EmitFootstep()
    {
        float radius = footstepBaseRadius;

        // louder footsteps when stamina is low
        if (health.StaminaRatio < 0.3f)
            radius += lowStaminaBonus;

        SoundEventBroadcaster.Broadcast(transform.position, radius);
    }
}
