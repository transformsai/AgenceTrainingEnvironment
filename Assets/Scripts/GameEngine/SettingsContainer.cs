using System;
using UnityEngine;
using UnityEngine.Serialization;
// ReSharper disable InconsistentNaming  // using json convention


[Serializable]
public class SettingsContainer
{


    [Header("McGuffinSettings")] 

    [Tooltip ("Miminum mass for McGuffins (when just planted)")]
    public float minMcGuffinMass = 0.1f;
    
    [Tooltip ("Miminum mass for McGuffins (when fully grown)")]
    public float maxMcGuffinMass = 5.5f;
    
    [Tooltip("Amount (out of 1.0) of size gain that the mcguffin grows each second it exists")]
    public float autoSizeGainPerSecond = 0.01f;

    [Tooltip("Amount (out of 1.0) of size gain that the mcguffin grows per second that it is being consumed")]
    public float sizeGainPerSecondConsumed = 0.045f;



    [Header ("Consume Settings")]


    [Tooltip ("How far away is considered in range for a Consume action to successfully take place.")]
    public float consumeMaxDistance = 0.8f;

    [Tooltip("Amount of time an agent needs to remain in a consume point to enter consume state.")]
    public double consumePointRemainDelay = 1;
    
    [Tooltip("Amount of time it takes for an agent to consume the mcg")]
    public float consumeDuration = 8f;

    


    [Header ("Movement Settings")]


    [Tooltip ("The Mass that the agents exert on the planet")]
    public float agentMass = 6;

    [Tooltip("Agent drag when not trying to move.")]
    public float brakeDrag = 8;

    [Tooltip("Agent drag when moving.")]
    public float moveDrag = 2;

    [Tooltip("Agent drag when falling or stunned.")]
    public float fallDrag = 0.02f;

    [Tooltip("Maximum speed agent can move at.")]
    public float maxSpeed = 10.16f;

    [Tooltip("Alters the agent's movement & animation speed.")]
    public float speedMultiplier =50 ;

    [Tooltip("Angle of the floor at which agents can switch to the sliding animation.")]
    public float slipFloorAngle = 60;
    
    [Tooltip("Agent y-velocity must be below this amount to count as falling.")]
    public float fallThreshold = 0.5f;
    
    [Tooltip("scaling for converting neural network outputs to movement actions.")]
    public float rlMovementMagnitude = 20;


    
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
    
    [Tooltip("Stamina to recover per second while sitting.")]
    [Range(0, 1)] public float staminaRecoveryRate = 0.5f;
    
    [Tooltip("Stamina cost per second of movement.")]
    [Range(0,1)] public float moveStaminaCost = 0.1f;

    

    [Header("Hold Settings")]


    [Tooltip("Agent must have this much stamina to begin holding on to the planet.")]
    [Range(0, 1)] public float minHoldStamina = 0.01f;
    
    [Tooltip("Seconds to hold the planet for before the agent can let go.")]
    public float minHoldDuration = 0.4f;

    [Tooltip("Stamina cost per second of holding on to the planet.")]
    [Range(0, 1)] public float holdStaminaCost = 0.1f;


    
    [Header ("Planet Settings")]
    

    [Tooltip("Angular (rotational) drag of the planet.")]
    public float angularDrag = 1.5f;
    
    [Tooltip("The mass of the planet")]
    public float planetMass = 12.89f;

    [Tooltip("The planet's max velocity (the max speed at which it can rotate).")]
    public float maxVelocity = 0.05f;

    [Tooltip("Angular acceleration above which to start rotating. Degrees per second.")]
    public float accelerationThreshold = 4;
    
    [Tooltip("Rate at which angular velocity goes to zero when below threshold. Degrees per second.")]
    [Range(0, 10f)]public float decelerationRate = 6;
    
    [Tooltip("The height relative to the top of the planet at which an object (Agent, seed, etc.) on the planet is considered to have fallen (and will be destroyed).")]
    public float killHeight = -35f;
}