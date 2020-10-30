using UnityEngine;

public abstract class AgentState : IState
{
    /// <summary>
    /// Checked prior to making a transition.
    /// </summary>
    public virtual bool CanEnter => true;

    public string Name { get; }

    protected SettingsContainer Settings => SettingsContainer.Instance;
    protected Transform Transform => Controller.transform;
    protected Rigidbody Rigidbody => Controller.rigidbody;
    protected AgentState CurrentState => Controller.CurrentState;

    public AgentController Controller;


    public AgentState(AgentController controller)
    {

        this.Controller = controller;
        this.Name = GetType().Name;
    }

    /// <summary>
    /// Called once, upon entering the state.
    /// </summary>
    public virtual void Enter() { }

    /// <summary>
    /// Called every frame while in the state.
    /// </summary>
    public virtual void Stay() { }

    /// <summary>
    /// Called once upong exiting the state.
    /// </summary>
    public virtual void Exit() { }

    /// <summary>
    /// Called every frame, even when not in the state.
    /// </summary>
    public virtual void GameUpdate() { }

    /// <summary>
    /// Called once, when the state machine is destroyed.
    /// </summary>
    public virtual void Dispose() { }
}
