using System;
using System.IO;
using UnityEngine;

[Serializable]
public class SettingsContainer
{

    
    //Singleton
    static SettingsContainer _instance = null;
    public static SettingsContainer Instance
    {
        get
        {
            if (_instance != null)
                return _instance;
            var filename = "EngineConfig.json";
            if (File.Exists(filename))
            {
                _instance = JsonUtility.FromJson<SettingsContainer>(File.ReadAllText(filename));
            }
            else
            {
                _instance = new SettingsContainer();
                File.WriteAllText(filename, JsonUtility.ToJson(_instance, true));
            }

            return _instance;
        }
    }


    public float RLMovementMagnitude = 20;

    
    [Tooltip("Agent y-velocity must be below this amount to count as falling.")]
    public float fallThreshold = 0.5f;

    [Header ("Rigidbody Settings")]
    public float AgentMass = 6;
    

    [Header ("Nurture Settings")]
    [Tooltip ("How far away is considered in range for a Nurture action to successfully take place.")]
    public float nurtureMaxDistance = 0.8f;
    [Tooltip("Amount of time an agent needs to remain in a consume point to enter consume state.")]
    public double ConsumePointRemainDelay = 1;
    
    [Header("Slip Settings")]
    [Tooltip("Angle of the floor at which agents can switch to the sliding animation.")]
    public float slipFloorAngle = 60;

    [Header ("Movement Settings")]
    [Tooltip("Agent drag when not trying to move.")]
    public float brakeDrag = 8;
    [Tooltip("Agent drag when moving.")]
    public float moveDrag = 2;
    [Tooltip("Agent drag when falling or stunned.")]
    public float fallDrag = 0.02f;
    [Tooltip("Maximum speed agent can move at.")]
    public float maxSpeed = 10.16f;
    [Tooltip("Stamina cost per second of movement.")]
    [Range(0,1)] public float moveStaminaCost = 0.1f;
    [Tooltip("Alters the agent's movement & animation speed.")]
    public float speedMultiplier =50 ;

    [Header ("Push Settings")]
    [Tooltip("Strength of the force to push agents with.")]
    public float pushForce = 85;
    [Tooltip("Seconds before agents can push again after having done it.")]
    public float pushCooldown = 1.6666f;
    [Tooltip("Height offset from agent feet to start spherecast.")]
    public float pushCastHeight = 0.75f;
    [Tooltip("Max distance of push raycast.")]
    public float pushDistance = 1.5f;
    [Tooltip("Seconds to stun agents that have been pushed.")]
    public float stunDuration = 7;

    [Header("Sit Settings")]
    [Tooltip("Seconds to sit for before the agent can get back up.")]
    public float minSitDuration = 2f;
    public float minHoldDuration = 0.4f;
    [Tooltip("Stamina to recover per second while sitting.")]
    [Range(0, 1)] public float staminaRecoveryRate = 0.5f;

    [Header("Hold Planet Settings")]
    [Tooltip("Agent must have this much stamina to begin holding on to the planet.")]
    [Range(0, 1)] public float minHoldStamina = 0.01f;
    [Tooltip("Stamina cost per second of holding on to the planet.")]
    [Range(0, 1)] public float holdStaminaCost = 0.1f;

    public float ConsumeDuration = 8f;

    [Header ("Planet Settings")]
    [Tooltip("Angular (rotational) drag of the planet.")]
    public float angularDrag = 1.5f;
    [Tooltip("The mass of the planet")]
    public float PlanetMass = 12.89f;
    [Tooltip("The planet's max velocity (the max speed at which it can rotate).")]
    public float maxVelocity = 0.05f;

    [Header("Rotation threshold settings")]
    [Tooltip("Angular acceleration above which to start rotating. Degrees per second.")]
    public float accelerationThreshold = 4;
    [Tooltip("Rate at which angular velocity goes to zero when below threshold. Degrees per second.")]
    [Range(0, 10f)]public float decelerationRate = 6;
    
    [Header ("Object Settings")]
    [Tooltip("The height relative to the top of the planet at which an object (Agent, seed, etc.) on the planet is considered to have fallen (and will be destroyed).")]
    public float killHeight = -30f;
}