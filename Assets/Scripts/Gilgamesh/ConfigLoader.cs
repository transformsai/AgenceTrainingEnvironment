using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LitJson;

static class ConfigLoader
{
    public static Dictionary<AgentNames, LoadedAgentConfig> LoadedConfigs = new Dictionary<AgentNames, LoadedAgentConfig>();

    public static LoadedAgentConfig GetConfig(AgentNames controllerAgentName)
    {
        LoadedAgentConfig loadedConfig;
        var isPreloaded = LoadedConfigs.TryGetValue(controllerAgentName, out loadedConfig);
        if (isPreloaded) return loadedConfig;
        return LoadConfigFromFile(controllerAgentName + ".AgentConfig.json");
    }

    private static LoadedAgentConfig LoadConfigFromFile(string fileName)
    {
        AgentConfig config;
        if (File.Exists(fileName))
        {
            config = JsonMapper.ToObject<AgentConfig>(File.ReadAllText(fileName));
        }
        else
        {
            config = DefaultConfig.Create();
            JsonWriter writer = new JsonWriter {PrettyPrint = true};
            JsonMapper.ToJson(config, writer);
            File.WriteAllText(fileName, writer.ToString());
        }
        return new LoadedAgentConfig(config);
    }
}

public class LoadedAgentConfig
{
    AgentConfig RawConfig;
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

