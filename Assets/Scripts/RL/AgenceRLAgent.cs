using System;
using System.Linq;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using UnityEngine;


public class AgenceRLAgent : Agent, IAgenceAgent
{
    public AgentController Controller;
    public LoadedAgentConfig Config;
    public World World => Controller.World;


    public Transform Transform => transform;
    public bool IsConsuming => Controller.CurrentState is ConsumingState;
    public bool IsGrounded => Controller.IsGrounded;
    public int IndexInPlanet { get; private set; }
    public AgenceRewardState RewardState => Controller.RewardState;
    public BehaviorParameters BrainParams;

    protected override void OnEnable()
    {
        Config = ConfigLoader.GetConfig(Controller.AgentName, false);
        var parameters = BrainParams.BrainParameters;
        parameters.VectorActionSize = new []{Config.Actions.Length};
        parameters.VectorObservationSize =
            Config.ObservationFunctions.Sum(it => it(this, World).Dimensions);

        base.OnEnable();
    }

    protected override void OnDisable()
    {
        Controller.RewardState.Died = true;
        ProcessRewards();
    }

    private void Start()
    {
        IndexInPlanet = World.Agents.IndexOf(Controller);
    }
    
    public void FixedUpdate()
    {
        ProcessRewards();
    }
    
    public void ProcessRewards()
    {
        for (int i = 0; i < Config.RewardFunctions.Length; i++)
        {
            var fn = Config.RewardFunctions[i];
            var scalar = Config.RewardScalars[i];
            if (scalar == 0) continue;
            var value = fn(this, World) * scalar;
            AddReward(value);
        }
        Controller.RewardState = new AgenceRewardState();
    }


    public override void CollectObservations(VectorSensor sensor)
    {
        foreach (var function in Config.ObservationFunctions)
        {
            var result = function(this, World);
            if (result.Dimensions > 0) sensor.AddObservation(result.x);
            if (result.Dimensions > 1) sensor.AddObservation(result.y);
            if (result.Dimensions > 2) sensor.AddObservation(result.z);
            if (result.Dimensions > 3) sensor.AddObservation(result.w);
        }

        base.CollectObservations(sensor);
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        var targetPositionX = 0f;
        var targetPositionZ = 0f;
        
        var controls = new CharacterControl();

        for (int i = 0; i < Config.Actions.Length; i++)
        {
            var action = Config.Actions[i];
            var value = vectorAction[i];
            switch (action)
            {
                case AgenceActions.MoveFwdBack: targetPositionX = value; break;
                case AgenceActions.MoveLeftRight: targetPositionZ = value; break;
                case AgenceActions.Consume: controls.Consume = true; break;
                case AgenceActions.Attack: controls.Headbutt = true; break;
                case AgenceActions.HoldPlanet: controls.HoldGround = true; break;
                case AgenceActions.Sit:controls.Sit = true; break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        controls.RelativeMovement = ConvertRLCoordinateToRelativeCoordinate(targetPositionX, targetPositionZ);

        Controller.Move(controls, ControlSources.MachineLearning);
    }

    private Vector2 ConvertRLCoordinateToRelativeCoordinate(float x, float z)
    {
        Vector3 movement = Vector3.zero;
        movement.x = x;
        movement.z = z;


        movement = movement * 0.5f;
        Quaternion rotation = Quaternion.Euler(movement);
        var relpos = transform.position - World.Centre;
        var targetPos = rotation * relpos;
        var absPos = targetPos + World.Centre;

        var relPoint = WorldToRelativePoint(absPos) * GameManager.Settings.rlMovementMagnitude;
        return relPoint;
    }

    public Vector2 WorldToRelativePoint(Vector3 pt)
    {
        var position = transform.position;
        var invVec = transform.InverseTransformVector(pt - position);
        return new Vector2(invVec.x, invVec.z);
    }
}
