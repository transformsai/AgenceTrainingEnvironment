using UnityEngine;

public class StunnedState : AgentState
{
    private float stunTimer = 0;
    public AgentController source;
    public float? lastStunTime = null;
    public float? overrideStunTimer = null;

    public StunnedState(AgentController controller) : base(controller) { }

    public override void Enter()
    {
        if (CurrentState != this) return;
        lastStunTime = Time.fixedTime;
        stunTimer = overrideStunTimer ?? Settings.stunDuration;
    }

    public override void Stay()
    {
        if (CurrentState != this) Exit();
        if (0 < stunTimer) stunTimer -= Time.deltaTime;
        else Controller.TryTransition<MovingState>();
    }


    public void ReceivePush(Vector3 pushDirection, Vector3 contactPoint, AgentController source)
    {

        Controller.rigidbody.AddForce(pushDirection * Settings.pushForce, ForceMode.Impulse);
        this.source = source;
        Controller.TryTransition(this);
    }

    public void Prepare(Vector3 pushDirection, Vector3 contactPoint, AgentController source)
    {

        Controller.rigidbody.AddForce(pushDirection * Settings.pushForce, ForceMode.Impulse);
        this.source = source;
        Controller.TryTransition(this);
    }


    public override void Exit()
    {
        source = null;
    }
}