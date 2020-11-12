using UnityEngine;

public class ConsumingState : AgentState
{
    public override bool CanEnter => _closestConsumePoint &&
                                     _closestConsumePoint.CanBeConsumed &&
    _timeInConsumePoint > Settings.consumePointRemainDelay;

    public float? LastConsumeTime = null;
    private float _timeInConsumePoint;

    private ConsumePoint _closestConsumePoint;
    public ConsumePoint ConsumeTarget = null;
    public ConsumePoint LastConsumedConsumePoint = null;

    private Planet Planet => Controller.World.Planet;

    private Vector3 _positionOnPlanet;

    private float _enterTime;

    public ConsumingState(AgentController controller) : base(controller) { }

    public override void GameUpdate()
    {

        var closestCP = Controller.World.GetClosestConsumePoint(Transform.position);
        if (_closestConsumePoint != closestCP) { _timeInConsumePoint = 0; }

        _closestConsumePoint = closestCP;

        if (_closestConsumePoint && ConsumeDistanceTo(_closestConsumePoint) < Settings.consumeMaxDistance)
            _timeInConsumePoint += Time.deltaTime;
        else
            _timeInConsumePoint = 0;

    }

    public override void Enter()
    {

        _enterTime = Time.time;
        // Use consume point
        ConsumeTarget = _closestConsumePoint;
        if (!ConsumeTarget.TryUsePoint(this))
        {
            Controller.TryTransition<MovingState>();
            return;
        }

        // Begin visuals


        LastConsumedConsumePoint = ConsumeTarget;

        // Store consuming position relative to planet
        _positionOnPlanet = Planet.transform.InverseTransformPoint(Rigidbody.position);
    }

    public override void Stay()
    {
        // Rotate to face target

        if (!ConsumeTarget || !Controller.IsGrounded || Controller.IsFloorSteep())
        {
            Controller.TryTransition<MovingState>();
            return;
        }
        
        // Lock consuming position
        Rigidbody.position = Planet.transform.TransformPoint(_positionOnPlanet);

        if (ConsumeProgress >= 1)
        {

            LastConsumeTime = Time.fixedTime;

            ConsumeTarget.ConsumeFinished();

            Controller.RewardState.DidConsume = true;

            Controller.TryTransition<MovingState>();
        }
    }

    public override void Exit()
    {
        if (ConsumeTarget) ConsumeTarget.ConsumeFinished();
        ConsumeTarget = null;
    }


    private float ConsumeDistanceTo(Component comp) =>
        Vector3.Distance(Controller.transform.position, comp.transform.position);

    public float ConsumeProgress => Mathf.Clamp01((Time.time - _enterTime) / ConsumeDuration);

    private static float ConsumeDuration => GameManager.Settings.consumeDuration;
}
