using UnityEngine;

public class PushingState : AgentState
{
    private int animPush;
    private float _pushCooldown = 0;
    public float SmackTime;
    private AgentController _target;

    public override bool CanEnter => 0 >= _pushCooldown && (_target = NearestAgent());

    public PushingState(AgentController controller) : base(controller)
    {

    }

    public override void GameUpdate()
    {
        if (0 < _pushCooldown) _pushCooldown -= Time.deltaTime;
    }

    public override void Enter()
    {
        if (CurrentState != this) return;
        if (!_target) return;

        if (_target.CurrentState != _target.consumingState)
        {
            _target.TryTransition<SittingState>();
        }
        _pushCooldown = Settings.pushCooldown;

        SmackTime = Time.time + .3f;

    }


    public override void Stay()
    {


        if (!_target)
        {

            Controller.TryTransition<MovingState>();
        }
        {
            if (SmackTime < Time.time) return;
            Controller.RewardState.DidPush = true;
            var sourcePoint = Transform.position + Transform.up * Settings.pushCastHeight;

            var targetCollider = _target.collider;

            var collisionPoint = targetCollider.ClosestPoint(sourcePoint);

            Vector3 directionToTarget = (_target.transform.position - Transform.position).normalized;
            _target.stunnedState.ReceivePush(directionToTarget, collisionPoint, Controller);

            Controller.TryTransition<MovingState>();
        }

    }


    private AgentController NearestAgent()
    {
        AgentController nearest = null;
        float leastSqDistance = float.MaxValue;
        foreach (var agent in Controller.World.Agents)
        {
            if (!agent || !agent.gameObject || agent == Controller) continue;
            Vector3 agentDir = agent.rigidbody.position - Rigidbody.position;
            float sqDist = agentDir.sqrMagnitude;
            if (sqDist > Settings.pushDistance) continue;
            if (leastSqDistance <= sqDist) continue;
            leastSqDistance = sqDist;
            nearest = agent;
        }

        return nearest;

    }
}
