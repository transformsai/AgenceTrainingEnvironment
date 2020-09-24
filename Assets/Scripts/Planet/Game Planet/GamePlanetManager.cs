using UnityEngine;

public class GamePlanetManager : BasePlanet
{
    public override void AditionalResets()
    {
        throw new System.NotImplementedException();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (var weight in weights)
        {
            if (weight.transform.position.y < -15)
            {
                Reset();
                Debug.Log("Weight Fell");
            }
        }

        foreach (var agent in agents)
        {
            if (agent.transform.position.y < -15)
            {
                Reset();
                Debug.Log("Agent fell");
                break;
            }

            Planet.UpdateWeightPosition(agent.rBody, agent.transform.position);
        }

        foreach (var weight in weights)
            Planet.UpdateWeightPosition(weight.GetComponent<Rigidbody>(), weight.transform.position);

        if (GetStartTime() + 300 < Time.fixedTime)
        {
            Reset();
            Debug.Log("Time ran out");
        }

        float totalAngle = 0;

        foreach (var weight in weights)
        {
            totalAngle += Vector3.Angle(weight.transform.position - Planet.transform.position, Vector3.up);
        }

        totalAngle = totalAngle / weights.Count;

        foreach (var agent in agents)
        {
            float stunnedRewardMod = 1;
            if (agent.IsStunned)
                stunnedRewardMod = 0.5f;

            agent.SetReward((1 - totalAngle / 45) * stunnedRewardMod * agent.health);

            if (agent.IsConsuming)
                agent.SetReward((1 + 1 - totalAngle / 45) * stunnedRewardMod * agent.health);
            else
            {
                foreach (var weight in weights)
                {
                    // if (Vector3.Distance(agent.transform.position, weight.transform.position) < 2f && weightTime + 10 < Time.fixedTime)
                    if (Vector3.Distance(agent.transform.position, weight.transform.position) < 2f)
                    {
                        var consumePoints = weight.consumePoints;
                        foreach (var consumePoint in consumePoints)
                        {
                            if (consumePoint.GetComponent<Collider>().bounds.Intersects(agent.GetComponent<Collider>().bounds) && consumePoint.CanConsume)
                            {
                                consumePoint.OnConsumption(agent);
                            }
                        }
                    }
                    //if (weightTime + 10 < Time.fixedTime)
                    agent.CanConsume();
                }
            }
        }

        GetTensorObs().ProcessOceanRewards(agents, weights, Planet);
    }
}
