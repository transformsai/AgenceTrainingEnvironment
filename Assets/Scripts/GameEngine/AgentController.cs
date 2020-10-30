using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using Random = UnityEngine.Random;


public class AgentController : MonoBehaviour, IStateMachine<AgentState>
{

    public World _world;
    public World World => _world ? _world : _world = GetComponentInParent<World>();

    
    private SettingsContainer Settings => SettingsContainer.Instance;

    // State
    public bool IsGrounded;

    [ReadOnly, SerializeField] private string StateName;

    public AgentState CurrentState
    {
        get => _currentState;
        set
        {
            StateName = value.Name;
            _currentState = value;
        }
    }

    public float Stamina
    {
        get => _stamina;
        set => _stamina = Mathf.Clamp01(value);
    }
    [SerializeField, ReadOnly] private float _stamina = 1;

    //Component Refs
    public new Rigidbody rigidbody;
    public new CapsuleCollider collider;
    public bool ShowDebugRays = false;
    
    private readonly ControlSourceHandler _controlSourceHandler = new ControlSourceHandler();
    public CharacterControl Control => _controlSourceHandler.GetControl();

    [Header("Ground Check")]
    private float GroundCheckDistance = 0.3f;
    private float RisingGroundCheckDistance = 0.1f;
    private float GroundCheckPullback = 0.15f;

    [Header("State")]
    [SerializeField] private Vector3 GroundNormal;
    [SerializeField] private Vector3 GroundGradient;
    [SerializeField] private Vector3 GroundTangentialVelocity;

    private readonly List<AgentState> _allStates = new List<AgentState>();
    private AgentState _currentState;

    public float StaminaCost => Time.fixedDeltaTime * Settings.moveStaminaCost;
    public bool HasFallen => transform.localPosition.y < SettingsContainer.Instance.killHeight;
    public bool IsFalling { get; private set; }



    public void Move(CharacterControl frameControls, ControlSources controlSource)
    {
        frameControls.RelativeMovement = Vector2.ClampMagnitude(frameControls.RelativeMovement, 1f);
        _controlSourceHandler.SubmitControls(frameControls, controlSource);
    }

    public bool TryTransition<T>(T t = default) where T : AgentState => StateMachineHelper.TryTransition(this, t);

    public T FindState<T>() where T : AgentState
    {
        foreach (var state in _allStates) if (state is T s) return s;
        return null;
    }


    public MovingState movingState;
    public StunnedState stunnedState;
    public PushingState pushingState;
    public ConsumingState consumingState;
    public SittingState sittingState;
    public HoldingPlanetState holdingPlanetState;

    public AgentNames AgentName;

    public AgenceRewardState RewardState;
    public Vector3 Normal;


    private void InitializeStates()
    {

        movingState = new MovingState(this);
        stunnedState = new StunnedState(this);
        pushingState = new PushingState(this);
        consumingState = new ConsumingState(this);
        sittingState = new SittingState(this);
        holdingPlanetState = new HoldingPlanetState(this);

        CurrentState = movingState;

        _allStates.Add(movingState); // Default state
        _allStates.Add(stunnedState);
        _allStates.Add(pushingState);
        _allStates.Add(consumingState);
        _allStates.Add(sittingState);
        _allStates.Add(holdingPlanetState);
    }

    private void ObserveGround()
    {
        if (!DetectGround(out var hitInfo)) return;

        var hitBody = hitInfo.rigidbody;
        GroundNormal = hitInfo.normal;
        GroundTangentialVelocity = hitBody ? hitBody.GetPointVelocity(hitInfo.point) : Vector3.zero;
        GroundGradient = Vector3.ProjectOnPlane(Vector3.up, hitInfo.normal);
        if (ShowDebugRays) Debug.DrawRay(hitInfo.point, GroundTangentialVelocity, Color.magenta);
        Normal = transform.InverseTransformDirection(GroundNormal);
    }

    private bool DetectGround(out RaycastHit hitInfo)
    {
        var isRising = (!IsGrounded && rigidbody.velocity.y > 0);

        var groundCheckDistance = isRising ? RisingGroundCheckDistance : GroundCheckDistance;

        // Start the ray from inside the character and cast down
        Ray ray = new Ray(transform.position + (transform.up * GroundCheckPullback), Vector3.down);

        // helper to visualize the ground check ray in the scene view
        if (ShowDebugRays) Debug.DrawLine(ray.origin, ray.origin + ray.direction * groundCheckDistance);

        // Test against the collidable world
        IsGrounded = World.Planet.collider.Raycast(ray, out hitInfo, groundCheckDistance);
        return IsGrounded;
    }

    private void PointUp()
    {
        if (!IsGrounded || CurrentState is HoldingPlanetState) return;
        ;
        Vector3 targetFwd = Vector3.ProjectOnPlane(rigidbody.rotation * Vector3.forward, GroundNormal);
        rigidbody.MoveRotation(Quaternion.Slerp(rigidbody.rotation, Quaternion.LookRotation(targetFwd, GroundNormal), 0.5f));
    }


    [SuppressMessage("ReSharper", "BitwiseOperatorOnEnumWithoutFlags")]
    private void Awake()
    {

        Stamina = Random.Range(0.8f, 1);
        if (!collider) collider = GetComponent<CapsuleCollider>();

        
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

        rigidbody.drag = Settings.brakeDrag;

        InitializeStates();
    }


    private void ApplyPlanetMovement()
    {
        if (!IsGrounded || Time.fixedDeltaTime <= 0) return;

        // Move with planet rotation
        rigidbody.MovePosition(rigidbody.position + GroundTangentialVelocity * Time.deltaTime);
    }

    private void DetectFall()
    {
        if (IsGrounded ||
            CurrentState is HoldingPlanetState ||
            rigidbody.velocity.y > Settings.fallThreshold)
        {
            IsFalling = false;
        }
        else
        {

            IsFalling = true;
        }

        var height = transform.position.y - World.transform.position.y;
        if (height < SettingsContainer.Instance.killHeight) Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        // Without this, FixedUpdate is called once even after disabling the script
        if (!enabled) return;

        rigidbody.mass = Settings.AgentMass;
        ObserveGround();

        if (!(CurrentState is HoldingPlanetState))
        {
            ApplyPlanetMovement();
        }

        StateMachineHelper.GameUpdateStates(_allStates);
        CurrentState.Stay();

        PointUp();
        DetectFall();
    }

    private void OnDestroy() => StateMachineHelper.DisposeStates(_allStates);

    public void UpdateMotion(Vector2 localMovement)
    {

        var cost = (localMovement.magnitude * .5f + .5f) * StaminaCost;

        if (IsGrounded && Time.fixedDeltaTime > 0)
        {
            Vector3 localForce = new Vector3(localMovement.x, 0, localMovement.y);
            var speedMultiplier = Settings.speedMultiplier;
            var force = transform.TransformDirection(localForce * speedMultiplier) / Time.fixedDeltaTime;
            // Move Along the surface of the planet
            var normal = transform.TransformDirection(Normal);
            var targetForce = Vector3.ProjectOnPlane(force, normal).normalized * force.magnitude;

            if (targetForce.sqrMagnitude > Mathf.Epsilon)
            {
                if (rigidbody.velocity.sqrMagnitude > Settings.maxSpeed * Settings.maxSpeed) targetForce *= 0;
                rigidbody.drag = Settings.moveDrag;
                rigidbody.AddForce(targetForce * Time.fixedDeltaTime);
                Stamina -= cost;
            }
            else
            {
                rigidbody.drag = Settings.brakeDrag;
            }

            if (ShowDebugRays) Debug.DrawRay(rigidbody.position, targetForce, Color.red);
        }
    }

    public bool IsFloorSteep()
    {
        Vector3 normal = transform.TransformDirection(Normal);
        return Vector3.Dot(normal, Vector3.up) < Mathf.Cos(Settings.slipFloorAngle * Mathf.Deg2Rad);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider != World.Planet.collider) return;
        rigidbody.useGravity = false;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider != World.Planet.collider) return;
        rigidbody.useGravity = true;
    }

}


