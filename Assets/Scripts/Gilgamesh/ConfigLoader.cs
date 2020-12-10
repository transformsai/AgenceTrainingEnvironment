using System.Collections.Generic;
using System.IO;
using LitJson;
using UnityEngine;

static class ConfigLoader
{
    private static Dictionary<AgentNames, LoadedAgentConfig> LoadedConfigs = new Dictionary<AgentNames, LoadedAgentConfig>();

    public static LoadedAgentConfig GetConfig(AgentNames controllerAgentName, bool forceReload)
    {
        if (!forceReload && LoadedConfigs.TryGetValue(controllerAgentName, out var loadedConfig))
            return loadedConfig;

        loadedConfig = LoadConfigFromFile(controllerAgentName + ".AgentConfig.json");

        LoadedConfigs[controllerAgentName] = loadedConfig;
        return loadedConfig;
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


    public static void SaveConfig(AgentNames controllerAgentName, AgentConfig config)
    {
        var fileName = controllerAgentName + ".AgentConfig.json";
        JsonWriter writer = new JsonWriter {PrettyPrint = true};
        JsonMapper.ToJson(config, writer);
        File.WriteAllText(fileName, writer.ToString());
        Debug.Log("Saved: "+ controllerAgentName );
        LoadedConfigs[controllerAgentName] = new LoadedAgentConfig(config);
    }
}