using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// base class for all AI states
public abstract class State
{
    public AIBrain brain;

    public State(AIBrain _brain)
    {
        brain = _brain;
    }

    public abstract void Enter();
    public abstract void Execute();
    public abstract void Exit();

    public override string ToString() => GetType().Name;
}
