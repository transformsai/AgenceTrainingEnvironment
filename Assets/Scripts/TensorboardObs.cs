using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class TensorboardObs : MonoBehaviour
{
    private StatsRecorder statsRecorder;
    // Start is called before the first frame update

    public void UpdateStatsRecorder(string key, float val)
    {
        statsRecorder = Academy.Instance.StatsRecorder;
        statsRecorder.Add(key, val);
    }

    public void ProcessOceanRewards(List<BaseAgent> agents, List<Weight> weights, GamePlanet planet)
    {
        foreach (var agent in agents)
        {
            float openReward = 0.1f;
            float concientiousReward = 9 - (System.Math.Abs(planet.AngularVelocity.x) + System.Math.Abs(planet.AngularVelocity.y) + System.Math.Abs(planet.AngularVelocity.z));
            float extrovertReward = 0.1f;
            float neuroticReward = 0;

            var agentAngle = Vector3.Angle(agent.transform.position - planet.transform.position, Vector3.up);
            neuroticReward = 1 - agentAngle / 45;

            foreach (var otherAgent in agents)
            {
                if (!agent.Equals(otherAgent) && otherAgent.IsConsuming)
                {
                    openReward = -1;
                    extrovertReward = 1;
                }
            }

            if (agent.IsConsuming)
                openReward = 1;

            UpdateStatsRecorder(agent.name + " Open", openReward);
            UpdateStatsRecorder(agent.name + " Concientious", concientiousReward);
            UpdateStatsRecorder(agent.name + " Extrovert", extrovertReward);
            UpdateStatsRecorder(agent.name + " Neurotic", neuroticReward);
            UpdateStatsRecorder(agent.name + " Open", openReward);
        }
    }
}
