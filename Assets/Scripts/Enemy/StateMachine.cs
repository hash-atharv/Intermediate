using System;

/// <summary>
/// Base class for a state owned by T (e.g. an Enemy). Override only what you need.
/// </summary>
public abstract class State<T>
{
    protected readonly T owner;
    protected readonly StateMachine<T> machine;

    protected State(T owner, StateMachine<T> machine)
    {
        this.owner = owner;
        this.machine = machine;
    }

    public virtual void Enter() { }
    public virtual void Tick() { }
    public virtual void Exit() { }
}

/// <summary>
/// Generic state machine. Works for any owner type (enemies, bosses, NPCs...).
/// States decide their own transitions by calling machine.ChangeState(...).
/// </summary>
public class StateMachine<T>
{
    public State<T> CurrentState { get; private set; }

    /// <summary>Raised after a transition with (previousState, newState).</summary>
    public event Action<State<T>, State<T>> OnStateChanged;

    public void Initialize(State<T> startState)
    {
        if (startState == null) return;

        CurrentState = startState;
        CurrentState.Enter();
    }

    public void ChangeState(State<T> newState)
    {
        if (newState == null || newState == CurrentState) return;

        State<T> previous = CurrentState;
        previous?.Exit();

        CurrentState = newState;
        CurrentState.Enter();

        OnStateChanged?.Invoke(previous, newState);
    }

    public void Tick()
    {
        CurrentState?.Tick();
    }
}