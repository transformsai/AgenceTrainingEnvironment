using UnityEngine;

public static class ObservationUtils
{
    public static ObservationResult NormalizedPosition(Vector3 worldPosition, IAgencePlanet planet)
    {
        return ObservationResult.InverseLerp(
            new Vector3(-0.5f, 0, -0.5f),
            new Vector3(0.5f, 0.6f, 0.5f),
            planet.Transform.rotation * (planet.Transform.InverseTransformPoint(worldPosition))
        );
    }

    public static ObservationResult NormalizedVelocity(Vector3 worldVelocity)
    {
        return ObservationResult.InverseLerp(
            new Vector3(-3, -3, -3),
            new Vector3(3, 3, 3),
            worldVelocity
        );
    }

    public static IAgenceAgent GetAgent(IAgenceAgent me, IAgencePlanet planet, int number)
    {
        int myIndex = me.IndexInPlanet;
        int targetIndex = (myIndex + number) % planet.Agents.Count;
        return planet.Agents[targetIndex];
    }

    public static ObservationResult GetAgentPosition(IAgenceAgent agent, IAgencePlanet planet, int targetAgentNumber)
    {
        var targetAgent = GetAgent(agent, planet, targetAgentNumber);
        return targetAgent.Exists() ? NormalizedPosition(targetAgent.Transform.position, planet) : Vector3.zero;
    }

    public static ObservationResult GetAgentVelocity(IAgenceAgent agent, IAgencePlanet planet, int targetAgentNumber)
    {
        var targetAgent = GetAgent(agent, planet, targetAgentNumber);
        return targetAgent.Exists()
            ? NormalizedVelocity(planet.Rigidbody.GetPointVelocity(targetAgent.Transform.position))
            : Vector3.zero;
    }

    public static ObservationResult GetMcgPosition(IAgencePlanet planet, int index)
    {
        index = index - 1;
        var targetMcg = index >= planet.McGuffins.Count ? null : planet.McGuffins[index];
        return targetMcg.Exists() ? NormalizedPosition(targetMcg.Transform.position, planet) : Vector3.zero;
    }

    public static IAgenceConsumePoint GetConsumePoint(IAgencePlanet planet, int mcgIndex, int consumePointIndex)
    {
        var targetMcg = mcgIndex >= planet.McGuffins.Count ? null : planet.McGuffins[mcgIndex];
        if (!targetMcg.Exists()) return null;
        return consumePointIndex >= targetMcg.ConsumePoints.Count ? null : targetMcg.ConsumePoints[consumePointIndex];

    }

    public static ObservationResult GetConsumePointPosition(IAgencePlanet planet, int mcgIndex, int consumePointIndex)
    {
        var consumePoint = GetConsumePoint(planet, mcgIndex, consumePointIndex);
        return consumePoint.Exists() ? NormalizedPosition(consumePoint.Transform.position, planet) : Vector3.zero;
    }

    public static ObservationResult GetConsumePointAvailability(IAgencePlanet planet, int mcgIndex, int consumePointIndex)
    {
        var consumePoint = GetConsumePoint(planet, mcgIndex, consumePointIndex);
        return consumePoint.Exists() && consumePoint.CanBeConsumed;
    }

    public static ObservationResult GetMcgVelocity(IAgencePlanet planet, int index)
    {
        var targetMcg = index >= planet.McGuffins.Count ? null : planet.McGuffins[index];
        return targetMcg.Exists() ? NormalizedVelocity(planet.Rigidbody.GetPointVelocity(targetMcg.Transform.position)) : Vector3.zero;
    }
}