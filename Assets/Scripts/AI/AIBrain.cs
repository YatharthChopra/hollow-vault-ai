using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class AIBrain : MonoBehaviour
{
    public Transform player;
    public TextMeshProUGUI stateLabel;

    public float patrolSpeed = 1.8f;
    public float chaseSpeed = 3.2f;

    public Transform[] patrolWaypoints;
    public float waypointReachedThreshold = 0.6f;

    [HideInInspector] public int waypointIndex = 0;
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public float stateEnterTime;
    [HideInInspector] public Vector3 lastKnownPlayerPos;

    private State currentState;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

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
            stateLabel.text = "State: " + currentState;
    }

    public void ChangeState(State newState)
    {
        if (currentState != null)
            currentState.Exit();

        currentState = newState;
        stateEnterTime = Time.time;
        currentState.Enter();
    }

    public float TimeInState()
    {
        return Time.time - stateEnterTime;
    }

    public void AdvanceWaypoint()
    {
        waypointIndex++;
        if (waypointIndex >= patrolWaypoints.Length) waypointIndex = 0;
    }
}
