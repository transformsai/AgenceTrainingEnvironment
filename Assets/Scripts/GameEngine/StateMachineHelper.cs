using System.Collections.Generic;

public static class StateMachineHelper
{
    public static bool TryTransition<TParent, TChild>(IStateMachine<TParent> stateMachine, TChild nextState = default)
        where TChild : TParent
        where TParent : IState
    {

        if (nextState == null) nextState = stateMachine.FindState<TChild>();
        if (!nextState.CanEnter) return false;

        //Debug.Log("Entering state: " + nextState.GetType().Name, stateMachine as Object);

        // Exit old state
        stateMachine.CurrentState?.Exit();

        // Enter new state
        stateMachine.CurrentState = nextState;
        nextState.Enter();
        return true;
    }

    public static void GameUpdateStates<T>(List<T> states) where T : IState
    {
        foreach (var state in states) state.GameUpdate();
    }

    public static void DisposeStates<T>(List<T> states) where T : IState
    {
        foreach (var state in states) state.Dispose();
    }
}