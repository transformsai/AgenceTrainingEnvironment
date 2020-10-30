using UnityEngine;

public class SittingState : AgentState
{

    private float sitTimer = 0;

    public SittingState(AgentController controller) : base(controller) { }

    public override void Enter()
    {
        if (CurrentState != this) return;

        sitTimer = Settings.minSitDuration;
    }

    public override void Stay()
    {
        if (CurrentState != this) Exit();

        Controller.Stamina += Settings.staminaRecoveryRate * Time.deltaTime;

        if (0 < sitTimer)
        {
            sitTimer -= Time.deltaTime;
        }
        else if (!Controller.Control.Sit)
        {
            Controller.TryTransition<MovingState>();
        }
    }

}
