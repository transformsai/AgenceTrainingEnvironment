using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

[System.Serializable]
public struct GroundObservations
{
    public Vector3 Normal;
    public Vector3 Gradient;
    public Vector3 TangentialVelocity;
}


public class StrippedAgentController : MonoBehaviour
{
    public float StaminaCost;
    public bool IsGrounded;
    public new Rigidbody rigidbody;
    public float Stamina;
    public float pushCastHeight;
    public float brakeDrag;
    public float moveDrag;
    public float pushForce;
    public float pushDistance;
    public new CapsuleCollider collider;
    public bool ShowDebugRays;
    public GamePlanet planet;
    public BasePlanet trainingPlanet;
    public float speedMultiplier;
    public GroundObservations RelativeObservations;
    public float maxSpeed;

    [Header("Ground Check")]
    protected float GroundCheckDistance = 5f;
    protected float RisingGroundCheckDistance = 0.1f;
    protected float GroundCheckPullback = 0.15f;

    [Header("State")]
    [SerializeField] protected Vector3 GroundNormal;
    [SerializeField] protected Vector3 GroundGradient;
    [SerializeField] protected Vector3 GroundTangentialVelocity;


    // Start is called before the first frame update
    [SuppressMessage("ReSharper", "BitwiseOperatorOnEnumWithoutFlags")]
    private void Start()
    {
        Stamina = Random.Range(0.8f, 1);
        if (!collider) collider = GetComponent<CapsuleCollider>();

        rigidbody = GetComponent<Rigidbody>();
        rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

        rigidbody.drag = brakeDrag;
    }

    public void UpdateMotion(Vector2 localMovement)
    {
        var cost = (localMovement.magnitude * .5f + .5f) * StaminaCost;

        if (IsGrounded && Time.fixedDeltaTime > 0)
        {
            Vector3 localForce = new Vector3(localMovement.x, 0, localMovement.y);
            var speedMultiplier = this.speedMultiplier;
            var force = transform.TransformDirection(localForce * speedMultiplier) / Time.fixedDeltaTime;
            // Move Along the surface of the planet
            var normal = transform.TransformDirection(RelativeObservations.Normal);
            var targetForce = Vector3.ProjectOnPlane(force, normal).normalized * force.magnitude;

            if (targetForce.sqrMagnitude > Mathf.Epsilon)
            {
                if (rigidbody.velocity.sqrMagnitude > maxSpeed * maxSpeed) targetForce *= 0;
                rigidbody.drag = moveDrag;
                rigidbody.AddForce(targetForce * Time.fixedDeltaTime);
                Stamina -= cost;
            }
            else
            {
                rigidbody.drag = brakeDrag;
            }

            if (ShowDebugRays) Debug.DrawRay(rigidbody.position, targetForce, Color.red);
        }
    }

    private void ObserveGround()
    {
        if (!DetectGround(out RaycastHit hitInfo)) return;

        var hitBody = hitInfo.rigidbody;
        GroundNormal = hitInfo.normal;
        GroundTangentialVelocity = hitBody ? hitBody.GetPointVelocity(hitInfo.point) : Vector3.zero;
        GroundGradient = Vector3.ProjectOnPlane(Vector3.up, hitInfo.normal);
        if (ShowDebugRays) Debug.DrawRay(hitInfo.point, GroundTangentialVelocity, Color.magenta);

        RelativeObservations = new GroundObservations
        {
            Normal = transform.InverseTransformDirection(GroundNormal),
            TangentialVelocity = transform.InverseTransformDirection(GroundTangentialVelocity),
            Gradient = transform.InverseTransformDirection(GroundGradient)
        };
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
        IsGrounded = planet.collider.Raycast(ray, out hitInfo, groundCheckDistance);
        return IsGrounded;
    }

    private void FixedUpdate()
    {
        // Without this, FixedUpdate is called once even after disabling the script
        if (!enabled) return;

        ObserveGround();

        ApplyPlanetMovement();

        PointUp();
        /*
        DetectFall();
        //AnimatorProxy.Stamina = Stamina;

        //var lastConsumeTime = consumingState.lastConsumeTime;
        //float illuminatedTime = Settings.WokeStateDuration;

        var remainingConsumeTime = lastConsumeTime == null ? 0
            : Mathf.Max(0, illuminatedTime - (Time.fixedTime - lastConsumeTime.Value));
        IsIlluminated = remainingConsumeTime > 0;
        AnimatorProxy.ConsumeState = remainingConsumeTime / illuminatedTime;
        */
    }

    private void ApplyPlanetMovement()
    {
        // Move with planet rotation
        rigidbody.MovePosition(rigidbody.position + GroundTangentialVelocity * Time.deltaTime);
    }

    private void PointUp()
    {
        Vector3 targetFwd = Vector3.ProjectOnPlane(rigidbody.rotation * Vector3.forward, GroundNormal);
        rigidbody.MoveRotation(Quaternion.LookRotation(targetFwd, GroundNormal));
    }

    private StrippedAgentController NearestAgent()
    {
        StrippedAgentController nearest = null;
        float leastSqDistance = float.MaxValue;
        foreach (var agent in trainingPlanet.agents)
        {
            if (!agent || !agent.gameObject || agent == this.GetComponent<BaseAgent>()) continue;
            Vector3 agentDir = agent.GetComponent<Rigidbody>().position - this.GetComponent<Rigidbody>().position;
            float sqDist = agentDir.sqrMagnitude;
            if (sqDist > pushDistance) continue;
            if (leastSqDistance <= sqDist) continue;
            leastSqDistance = sqDist;
            nearest = agent.GetComponent<StrippedAgentController>();
        }

        return nearest;
    }

    public void Push()
    {
        var target = NearestAgent();

        if (!target)
            return;

        Debug.Log("agent push");

        var sourcePoint = this.transform.position + this.transform.up * pushCastHeight;

        var targetCollider = target.collider;

        var collisionPoint = targetCollider.ClosestPoint(sourcePoint);

        Vector3 directionToTarget = (target.transform.position - this.transform.position).normalized;

        target.GetComponent<Rigidbody>().AddForce(directionToTarget * pushForce, ForceMode.Impulse);
    }
}
