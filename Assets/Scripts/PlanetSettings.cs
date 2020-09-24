using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Agence Settings/Planet Settings")]
public class PlanetSettings : ScriptableObject
{
    [Header ("Planet Settings")]
    [Tooltip("Angular (rotational) drag of the planet.")]
    public float angularDrag;
    [Tooltip("The mass of the planet")]
    public float mass;
    [Tooltip("The planet's max velocity (the max speed at which it can rotate).")]
    public float maxVelocity;
    [Tooltip("The planet's radius.")]
    public float radius;

    [Header("Rotation threshold settings")]
    [Tooltip("Angular acceleration above which to start rotating. Degrees per second.")]
    public float accelerationThreshold = 0;
    [Tooltip("Rate at which angular velocity goes to zero when below threshold. Degrees per second.")]
    [Range(0, 10f)]public float decelerationRate = 2;
    
    [Header ("Object Settings")]
    [Tooltip("The height relative to the top of the planet at which an object (Agent, seed, etc.) on the planet is considered to have fallen (and will be destroyed).")]
    public float killHeight = -30f;
}
