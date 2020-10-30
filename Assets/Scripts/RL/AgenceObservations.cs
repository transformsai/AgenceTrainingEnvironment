using UnityEngine;
using static ObservationUtils;



public static class AgenceObservations
{

    public static ObservationResult MyPosition(IAgenceAgent agent, IAgencePlanet planet) => NormalizedPosition(agent.Transform.position, planet);

    public static ObservationResult PlanetAngularVelocity(IAgenceAgent agent, IAgencePlanet planet) => NormalizedVelocity(planet.Rigidbody.angularVelocity);

    public static ObservationResult PlanetTangentialVelocityAtMyFeet(IAgenceAgent agent, IAgencePlanet planet) => NormalizedVelocity(planet.Rigidbody.GetPointVelocity(agent.Transform.position));

    public static ObservationResult LightningTargetPosition(IAgenceAgent agent, IAgencePlanet planet) =>
        planet.Lightning ? NormalizedPosition(planet.Lightning.transform.position, planet) : Vector3.zero;

    // TODO: Port state machine from main repo
    public static ObservationResult CanConsume(IAgenceAgent agent, IAgencePlanet planet) => false;
    public static ObservationResult IsSitting(IAgenceAgent agent, IAgencePlanet planet) => false;
    public static ObservationResult CanPush(IAgenceAgent agent, IAgencePlanet planet) => false;
    public static ObservationResult IsPushing(IAgenceAgent agent, IAgencePlanet planet) => false;
    // --

    public static ObservationResult IsConsuming(IAgenceAgent agent, IAgencePlanet planet) => agent.IsConsuming;

    public static ObservationResult IsGrounded(IAgenceAgent agent, IAgencePlanet planet) => agent.IsGrounded;


    public static ObservationResult Agent1Position(IAgenceAgent agent, IAgencePlanet planet) => GetAgentPosition(agent, planet, 1);

    public static ObservationResult Agent2Position(IAgenceAgent agent, IAgencePlanet planet) => GetAgentPosition(agent, planet, 2);

    public static ObservationResult Agent3Position(IAgenceAgent agent, IAgencePlanet planet) => GetAgentPosition(agent, planet, 3);

    public static ObservationResult Agent4Position(IAgenceAgent agent, IAgencePlanet planet) => GetAgentPosition(agent, planet, 4);

    public static ObservationResult Agent1TangentialVelocityAtFeet(IAgenceAgent agent, IAgencePlanet planet) => GetAgentVelocity(agent, planet, 1);

    public static ObservationResult Agent2TangentialVelocityAtFeet(IAgenceAgent agent, IAgencePlanet planet) => GetAgentVelocity(agent, planet, 2);

    public static ObservationResult Agent3TangentialVelocityAtFeet(IAgenceAgent agent, IAgencePlanet planet) => GetAgentVelocity(agent, planet, 3);

    public static ObservationResult Agent4TangentialVelocityAtFeet(IAgenceAgent agent, IAgencePlanet planet) => GetAgentVelocity(agent, planet, 4);

    public static ObservationResult McGuffin1Position(IAgenceAgent agent, IAgencePlanet planet) => GetMcgPosition(planet, 1);

    public static ObservationResult McGuffin2Position(IAgenceAgent agent, IAgencePlanet planet) => GetMcgPosition(planet, 2);

    public static ObservationResult McGuffin3Position(IAgenceAgent agent, IAgencePlanet planet) => GetMcgPosition(planet, 3);

    public static ObservationResult McGuffin1Velocity(IAgenceAgent agent, IAgencePlanet planet) => GetMcgVelocity(planet, 1);

    public static ObservationResult McGuffin2Velocity(IAgenceAgent agent, IAgencePlanet planet) => GetMcgVelocity(planet, 2);

    public static ObservationResult McGuffin3Velocity(IAgenceAgent agent, IAgencePlanet planet) => GetMcgVelocity(planet, 3);

    public static ObservationResult McGuffin1Flower1Position(IAgenceAgent agent, IAgencePlanet planet) => GetConsumePointPosition(planet, 1, 1);
    public static ObservationResult McGuffin1Flower2Position(IAgenceAgent agent, IAgencePlanet planet) => GetConsumePointPosition(planet, 1, 2);
    public static ObservationResult McGuffin1Flower3Position(IAgenceAgent agent, IAgencePlanet planet) => GetConsumePointPosition(planet, 1, 3);
    public static ObservationResult McGuffin2Flower1Position(IAgenceAgent agent, IAgencePlanet planet) => GetConsumePointPosition(planet, 2, 1);
    public static ObservationResult McGuffin2Flower2Position(IAgenceAgent agent, IAgencePlanet planet) => GetConsumePointPosition(planet, 2, 2);
    public static ObservationResult McGuffin2Flower3Position(IAgenceAgent agent, IAgencePlanet planet) => GetConsumePointPosition(planet, 2, 3);
    public static ObservationResult McGuffin3Flower1Position(IAgenceAgent agent, IAgencePlanet planet) => GetConsumePointPosition(planet, 3, 1);
    public static ObservationResult McGuffin3Flower2Position(IAgenceAgent agent, IAgencePlanet planet) => GetConsumePointPosition(planet, 3, 2);
    public static ObservationResult McGuffin3Flower3Position(IAgenceAgent agent, IAgencePlanet planet) => GetConsumePointPosition(planet, 3, 3);
    public static ObservationResult McGuffin1Flower1Availability(IAgenceAgent agent, IAgencePlanet planet) => GetConsumePointAvailability(planet, 1, 1);
    public static ObservationResult McGuffin1Flower2Availability(IAgenceAgent agent, IAgencePlanet planet) => GetConsumePointAvailability(planet, 1, 2);
    public static ObservationResult McGuffin1Flower3Availability(IAgenceAgent agent, IAgencePlanet planet) => GetConsumePointAvailability(planet, 1, 3);
    public static ObservationResult McGuffin2Flower1Availability(IAgenceAgent agent, IAgencePlanet planet) => GetConsumePointAvailability(planet, 2, 1);
    public static ObservationResult McGuffin2Flower2Availability(IAgenceAgent agent, IAgencePlanet planet) => GetConsumePointAvailability(planet, 2, 2);
    public static ObservationResult McGuffin2Flower3Availability(IAgenceAgent agent, IAgencePlanet planet) => GetConsumePointAvailability(planet, 2, 3);
    public static ObservationResult McGuffin3Flower1Availability(IAgenceAgent agent, IAgencePlanet planet) => GetConsumePointAvailability(planet, 3, 1);
    public static ObservationResult McGuffin3Flower2Availability(IAgenceAgent agent, IAgencePlanet planet) => GetConsumePointAvailability(planet, 3, 2);
    public static ObservationResult McGuffin3Flower3Availability(IAgenceAgent agent, IAgencePlanet planet) => GetConsumePointAvailability(planet, 3, 3);
}
