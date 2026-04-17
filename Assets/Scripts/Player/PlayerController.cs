using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerHealth))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 4f;
    public float gravity = -9.81f;
    public float footstepInterval = 0.4f;
    public float footstepBaseRadius = 3f;
    public float lowStaminaBonus = 5f;

    CharacterController controller;
    PlayerHealth health;
    float nextFootstepTime;
    Vector3 velocity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        health = GetComponent<PlayerHealth>();
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(h, 0f, v) * moveSpeed;

        velocity.y += gravity * Time.deltaTime;
        move.y = velocity.y;

        controller.Move(move * Time.deltaTime);

        // face movement direction
        Vector3 hMove = new Vector3(move.x, 0f, move.z);
        if (hMove.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(hMove);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 15f * Time.deltaTime);
        }

        // emit footstep sound while moving
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

        // louder when stamina is low
        if (health.StaminaRatio < 0.3f)
            radius += lowStaminaBonus;

        GameEvents.OnSoundEmitted?.Invoke(transform.position, radius / footstepBaseRadius);
    }
}
