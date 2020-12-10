using System;
using System.Linq;

public class LoadedAgentConfig
{
    public AgentConfig RawConfig;
    public readonly ObservationFunction[] ObservationFunctions;
    public readonly AgenceActions[] Actions;
    public readonly RewardFunction[] RewardFunctions;
    public readonly float[] RewardScalars;

    public LoadedAgentConfig(AgentConfig config)
    {
        RawConfig = config;
        ObservationFunctions = config.Observations.Select(it => DefaultConfig.AllObservations[it]).ToArray();
        Actions = config.Actions.Select(it => (AgenceActions) Enum.Parse(typeof(AgenceActions), it)).ToArray();
        RewardFunctions = config.Rewards.Select(it => DefaultConfig.AllRewards[it.Key]).ToArray();
        RewardScalars = config.Rewards.Select(it => config.Rewards[it.Key]).ToArray();
    }
}