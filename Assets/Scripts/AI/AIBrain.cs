using UnityEngine;
using UnityEngine.AI;
using TMPro;

/// <summary>
/// Central AI controller / state machine host shared by both the Crypt Sentinel
/// and The Shade.  Individual behaviour is expressed through the active State.
///
/// Attach this (or a subclass) to any AI agent in the scene.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class AIBrain : MonoBehaviour
{
    // -------------------------------------------------------------------------
    // Inspector-exposed scene references
    // -------------------------------------------------------------------------

    [Header("Scene References")]
    [Tooltip("The player's Transform.")]
    public Transform player;

    [Tooltip("Optional UI label that shows the current state name (for debugging).")]
    public TextMeshProUGUI stateLabel;

    // -------------------------------------------------------------------------
    // Shared config – subclasses can override defaults in Awake
    // -------------------------------------------------------------------------

    [Header("Movement")]
    public float patrolSpeed   = 1.8f;
    public float chaseSpeed    = 3.2f;

    [Header("Waypoints")]
    public Transform[] patrolWaypoints;
    [HideInInspector] public int waypointIndex = 0;
    public float waypointReachedThreshold = 0.6f;

    // -------------------------------------------------------------------------
    // Shared runtime state
    // -------------------------------------------------------------------------

    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public float       stateEnterTime;   // Time.time when state was entered
    [HideInInspector] public Vector3     lastKnownPlayerPos;

    private State currentState;

    // -------------------------------------------------------------------------
    // Lifecycle
    // -------------------------------------------------------------------------

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Subclasses call this in their own Start / Awake to set initial state
    protected void InitialState(State startState)
    {
        currentState = startState;
        stateEnterTime = Time.time;
        currentState.Enter();
    }

    protected virtual void Update()
    {
        if (currentState == null) return;
        currentState.Execute();

        if (stateLabel != null)
            stateLabel.text = $"State: {currentState}";
    }

    // -------------------------------------------------------------------------
    // Public API
    // -------------------------------------------------------------------------

    /// <summary>Transitions to a new state, calling Exit on the old and Enter on the new.</summary>
    public void ChangeState(State newState)
    {
        if (currentState != null)
        {
            Debug.Log($"[{gameObject.name}] {currentState} → {newState}");
            currentState.Exit();
        }

        currentState = newState;
        stateEnterTime = Time.time;
        currentState.Enter();
    }

    /// <summary>Convenience: seconds elapsed since entering the current state.</summary>
    public float TimeInState() => Time.time - stateEnterTime;

    /// <summary>Helper: advance patrol index, wrapping around.</summary>
    public void AdvanceWaypoint()
    {
        waypointIndex = (waypointIndex + 1) % patrolWaypoints.Length;
    }
}
