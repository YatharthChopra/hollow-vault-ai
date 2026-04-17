using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

// The Shade — spectral stalker that reacts to sound and light
// Owns the FSM and switches between: Drift, Alert, Stalk, Hunt, Retreat, Rallied
[RequireComponent(typeof(NavMeshAgent))]
public class ShadeBrain : MonoBehaviour
{
    [Header("References")]
    public Transform playerTransform;
    public Transform[] patrolWaypoints;
    public TextMeshProUGUI stateLabel;

    [Header("Movement")]
    public float driftSpeed = 2.5f;
    public float huntSpeed = 4.5f;

    [Header("Detection")]
    public float hearingRadius = 8f;
    public float lightDetectRadius = 5f;
    public float alertTimeout = 3f;
    public float huntRange = 3f;
    public float escapeRange = 5f;

    [Header("Attack")]
    public float staminaDrainRate = 10f;
    public float rallyRadius = 10f;

    // runtime values states can read/write
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public Vector3 lastKnownSoundPos;
    [HideInInspector] public int waypointIndex = 0;

    // all six states
    [HideInInspector] public ShadeDriftState    driftState;
    [HideInInspector] public ShadeAlertState    alertState;
    [HideInInspector] public ShadeStalkState    stalkState;
    [HideInInspector] public ShadeHuntState     huntState;
    [HideInInspector] public ShadeRetreatState  retreatState;
    [HideInInspector] public ShadeRalliedState  ralliedState;

    State currentState;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        driftState   = new ShadeDriftState(this);
        alertState   = new ShadeAlertState(this);
        stalkState   = new ShadeStalkState(this);
        huntState    = new ShadeHuntState(this);
        retreatState = new ShadeRetreatState(this);
        ralliedState = new ShadeRalliedState(this);
    }

    void Start()
    {
        // listen for the Sentinel's rallying cry
        GameEvents.OnSentinelRallyingCry += OnRallyCry;
        ChangeState(driftState);
    }

    void OnDestroy()
    {
        GameEvents.OnSentinelRallyingCry -= OnRallyCry;
    }

    void Update()
    {
        if (currentState != null)
        {
            currentState.Execute();

            if (stateLabel != null)
                stateLabel.text = "Shade: " + currentState;
        }
    }

    public void ChangeState(State newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }

    // Sentinel fired the rallying cry — check if we are close enough to respond
    void OnRallyCry(Vector3 sentinelPos)
    {
        float dist = Vector3.Distance(transform.position, sentinelPos);
        if (dist <= rallyRadius)
            ChangeState(ralliedState);
    }

    public void AdvanceWaypoint()
    {
        waypointIndex++;
        if (waypointIndex >= patrolWaypoints.Length) waypointIndex = 0;
    }
}
