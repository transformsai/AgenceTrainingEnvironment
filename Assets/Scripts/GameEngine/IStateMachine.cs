public interface IStateMachine<T> where T : IState
{
    T CurrentState { get; set; }
    bool TryTransition<S>(S s = default) where S : T;
    S FindState<S>() where S : T;
}


public interface IState
{
    /// <summary>
    /// Checked prior to making a transition.
    /// </summary>
    bool CanEnter { get; }

    /// <summary>
    /// Called once, upon entering the state.
    /// </summary>
    void Enter();

    /// <summary>
    /// Called every frame while in the state.
    /// </summary>
    void Stay();

    /// <summary>
    /// Called once upong exiting the state.
    /// </summary>
    void Exit();

    /// <summary>
    /// Called every frame, even when not in the state.
    /// </summary>
    void GameUpdate();

    /// <summary>
    /// Called once, when the state machine is destroyed.
    /// </summary>
    void Dispose();
}