using UnityEngine;

public class MovingState : AgentState
{
    public MovingState(AgentController controller) : base(controller) { }
    private float StaminaCost => Time.fixedDeltaTime * Settings.moveStaminaCost;

    public override void GameUpdate()
    {
        if (Controller.IsGrounded || (Controller.CurrentState is HoldingPlanetState)) return;
        Rigidbody.drag = Settings.fallDrag;
    }


    public override void Stay()
    {
        CharacterControl control = Controller.Control;

        bool push = control.Headbutt;
        Vector2 localMove = control.RelativeMovement; 
        bool consume = control.Consume;
        bool sit = control.Sit;
        bool holdGround = control.HoldGround;
        
        Debug.DrawRay(Transform.position, Transform.TransformVector(new Vector3(localMove.x,0,localMove.y)) * 100, Color.yellow);

        if (StaminaCost >= Controller.Stamina) localMove = Vector2.zero;

        bool isSlipping = Controller.IsFloorSteep();

        // Move
        Controller.UpdateMotion(localMove);

        // Sit
        if (sit) Controller.TryTransition<SittingState>();

        // Hold Ground
        if (holdGround) Controller.TryTransition<HoldingPlanetState>();
        
        // Push
        if (push && !isSlipping) Controller.TryTransition<PushingState>();


        // Consume
        if (consume && !isSlipping) Controller.TryTransition<ConsumingState>();
    }

    public override void Exit()
    {
        Controller.UpdateMotion(Vector2.zero);
    }
}