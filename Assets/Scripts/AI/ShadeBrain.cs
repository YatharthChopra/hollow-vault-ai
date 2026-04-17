using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadeBrain : AIBrain
{
    public float driftSpeed = 2.5f;
    public float huntSpeed = 4.5f;
    public float alertTimeout = 3f;
    public float huntRange = 3f;
    public float escapeRange = 5f;
    public float staminaDrainRate = 10f;

    [HideInInspector] public HearingSensor hearing;
    [HideInInspector] public LightSensor lightSense;

    protected override void Awake()
    {
        base.Awake();
        hearing = GetComponent<HearingSensor>();
        lightSense = GetComponent<LightSensor>();

        patrolSpeed = driftSpeed;
        chaseSpeed = huntSpeed;

        InitialState(new ShadeDriftState(this));
    }

    protected override void Update()
    {
        hearing.ResetFrame();
        base.Update();
    }

    // called by the Sentinel when it does its rallying cry
    public void ReceiveRally(Vector3 targetPosition)
    {
        lastKnownPlayerPos = targetPosition;
        ChangeState(new ShadeRalliedState(this));
    }
}
