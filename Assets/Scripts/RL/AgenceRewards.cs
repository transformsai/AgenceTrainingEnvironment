public static class AgenceRewards
{
    public static float SurvivalTick(IAgenceAgent agent, IAgencePlanet planet) => 1;
    public static float PushCompleted(IAgenceAgent agent, IAgencePlanet planet) => agent.RewardState.DidPush ? 1 : 0;
    public static float ConsumeTick(IAgenceAgent agent, IAgencePlanet planet) => agent.IsConsuming ? 1 : 0;
    public static float ConsumeCompleted(IAgenceAgent agent, IAgencePlanet planet) => agent.RewardState.DidConsume ? 1 : 0;
    public static float ThisAgentDied(IAgenceAgent agent, IAgencePlanet planet) => agent.RewardState.Died ? 1 : 0;

}

