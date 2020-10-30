using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Planet : MonoBehaviour
{
    public new Collider collider;
    public Rigidbody InertialWorld;
    private SettingsContainer settings => SettingsContainer.Instance;
    public float Radius;
    public new Rigidbody rigidbody;


    public Vector3 ZenithTorque => Vector3.Cross(lastFrameTorque, Vector3.up);
    public Vector3 ZenithVelocity => Vector3.Cross(AngularVelocity, Vector3.up);

    [Tooltip("Objects on these layers will not weigh down on the world.")]
    [SerializeField] private LayerMask weightExclusionMask;
    [Tooltip("Angular velocity to start with.")]
    [SerializeField] private Vector3 initialAngularVelocity;

    public Vector3 AngularVelocity => rigidbody.angularVelocity;

    private readonly Dictionary<Rigidbody, Vector3> weights = new Dictionary<Rigidbody, Vector3>(40, ObjectComparer.Instance);
    private Vector3 torque;
    public Vector3 lastFrameTorque;
    public bool IsBalanced { get; set; }


    public void AddWeight(Rigidbody weight, Vector3 position) => weights.Add(weight, position);

    public void UpdateWeightPosition(Rigidbody weight, Vector3 position)
    {
        if (!weights.ContainsKey(weight)) return;
        weights[weight] = position;
    }

    class ObjectComparer : IEqualityComparer<Object>
    {
        public static ObjectComparer Instance = new ObjectComparer();

        public bool Equals(Object x, Object y)
        {
            return x == y;
        }

        public int GetHashCode(Object obj)
        {
            return obj.GetHashCode();
        }
    }
    public void RemoveWeight(Rigidbody weight) => weights.Remove(weight);

    private void Awake() => rigidbody = GetComponent<Rigidbody>();

    private void FixedUpdate()
    {
        // Apply settings
        Radius = transform.TransformVector(Vector3.right).magnitude;
        InertialWorld.angularDrag = settings.angularDrag;
        InertialWorld.mass = settings.PlanetMass;
        InertialWorld.maxAngularVelocity = settings.maxVelocity;

        // Add torque from non-colliding weights
        foreach (var kvp in weights)
        {
            var rb = kvp.Key;
            if (!rb) continue;

            var gravity = Physics.gravity * rb.mass;
            torque += Vector3.Cross(kvp.Value, gravity);
        }

        UpdateVelocity();

        rigidbody.MoveRotation(InertialWorld.rotation);
    }

    private void OnCollisionStay(Collision other)
    {
        if (0 != (1 << other.gameObject.layer & weightExclusionMask.value)) return;

        var rb = other.rigidbody;

        if (rb == null) return;
        if (weights.ContainsKey(rb)) return;
        if (other.contactCount == 0) return;

        Vector3 sum = Vector3.zero;
        for (int i = 0; i < other.contactCount; i++)
        {
            ContactPoint contact = other.GetContact(i);
            sum += contact.point - InertialWorld.position;
        }

        var avg = sum / other.contactCount;

        Vector3 impulse = -other.impulse;

        var gravity = Physics.gravity * rb.mass;

        torque += Vector3.Cross(avg, impulse + gravity);
    }

    private void UpdateVelocity()
    {
        // Calculate angular acceleration from torque: a = T/I
        Vector3 momentOfInertia = InertialWorld.inertiaTensorRotation * InertialWorld.inertiaTensor;
        Vector3 invMomentOfInertia = new Vector3(1 / momentOfInertia.x, 1 / momentOfInertia.y, 1 / momentOfInertia.z);
        float angularAccelerationMagnitude = Vector3.Scale(torque, invMomentOfInertia).magnitude;

        if (angularAccelerationMagnitude * Mathf.Rad2Deg > settings.accelerationThreshold)
        {
            // Only apply torque if acceleration is high enough
            InertialWorld.AddTorque(torque);
            IsBalanced = false;
        }
        else
        {
            // Delecerate to zero velocity when acceleration from torque is too low
            Vector3 deceleration = (Vector3.zero - InertialWorld.angularVelocity).normalized * settings.decelerationRate * Mathf.Deg2Rad;
            InertialWorld.AddTorque(deceleration, ForceMode.Acceleration);
            IsBalanced = true;
        }

        lastFrameTorque = torque;
        torque = Vector3.zero;
    }


    public void Restart()
    {
        InertialWorld.angularVelocity = initialAngularVelocity;
        InertialWorld.MoveRotation(Quaternion.identity);
    }

    public void OnHoverExit()
    {

    }

    public void OnHoverEnter()
    {

    }
}