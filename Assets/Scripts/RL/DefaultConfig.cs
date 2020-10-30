using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

static class DefaultConfig
{
    public static readonly Dictionary<string, ObservationFunction> AllObservations = new Dictionary<string, ObservationFunction>();
    public static readonly Dictionary<string, RewardFunction> AllRewards = new Dictionary<string, RewardFunction>();
    public static readonly string[] AllActions;


    static DefaultConfig()
    {
        var obsMethods = typeof(AgenceObservations).GetMethods(BindingFlags.Static | BindingFlags.Public);
        foreach (var method in obsMethods)
        {
            AllObservations[method.Name] = (ObservationFunction)method.CreateDelegate(typeof(ObservationFunction));
        }

        var rewardMethods = typeof(AgenceRewards).GetMethods(BindingFlags.Static | BindingFlags.Public);
        foreach (var method in rewardMethods)
        {
            AllRewards[method.Name] = (RewardFunction)method.CreateDelegate(typeof(RewardFunction));
        }

        AllActions = Enum.GetNames(typeof(AgenceActions)).ToArray();
    }

    public static AgentConfig Create()
    {
        var result = new AgentConfig
        {
            Observations = AllObservations.Keys.ToList(),
            Actions = AllActions.ToList(),
            Rewards = AllRewards.ToDictionary(it=>it.Key, it=>0f)
        };

        result.Rewards[nameof(AgenceRewards.ConsumeTick)] = 1f;
        return result;
    }

}
