using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class CryptSentinel : AIBrain
{
    public float investigateTimeout = 6f;
    public float staggerDuration = 1.5f;
    public float combatRange = 2.0f;
    public float rallyRadius = 10f;
    public float memoryDuration = 4f;

    public float maxHP = 100f;
    public float rallyHPThreshold = 50f;
    [HideInInspector] public float currentHP;

    public GameObject alertIcon;
    public GameObject questionIcon;

    public ShadeBrain shade;

    [HideInInspector] public VisionSensor vision;
    [HideInInspector] public HearingSensor hearing;
    [HideInInspector] public bool hasRallied = false;

    protected override void Awake()
    {
        base.Awake();
        vision = GetComponent<VisionSensor>();
        hearing = GetComponent<HearingSensor>();
        currentHP = maxHP;

        InitialState(new SentinelPatrolState(this));
    }

    protected override void Update()
    {
        hearing.ResetFrame();
        base.Update();
    }

    public void TakeDamage(float amount)
    {
        currentHP -= amount;
        if (currentHP < 0) currentHP = 0;

        // always stagger when hit
        ChangeState(new SentinelStaggeredState(this));
    }

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

    public void ShowAlertIcon(bool show)
    {
        if (alertIcon != null) alertIcon.SetActive(show);
    }

    public void ShowQuestionIcon(bool show)
    {
        if (questionIcon != null) questionIcon.SetActive(show);
    }
}
