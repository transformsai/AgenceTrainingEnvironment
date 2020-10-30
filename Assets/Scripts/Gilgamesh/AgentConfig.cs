using System;
using System.Collections.Generic;

[Serializable]
public class AgentConfig
{
    public List<string> Observations = new List<string>();
    public List<string> Actions = new List<string>();
    public Dictionary<string, float> Rewards = new Dictionary<string, float>();
}
