/// <summary>
/// Abstract base class for all FSM states.
/// Each AI agent (Sentinel, Shade) has its own concrete state implementations
/// that inherit from this class. Pattern: polymorphic state machine.
/// </summary>
public abstract class State
{
    /// <summary>Reference to the state machine that owns this state.</summary>
    protected AIBrain brain;

    public State(AIBrain _brain)
    {
        brain = _brain;
    }

    /// <summary>Called once when the state is entered.</summary>
    public abstract void Enter();

    /// <summary>Called every frame while this state is active.</summary>
    public abstract void Execute();

    /// <summary>Called once when the state is exited.</summary>
    public abstract void Exit();

    public override string ToString() => GetType().Name;
}
