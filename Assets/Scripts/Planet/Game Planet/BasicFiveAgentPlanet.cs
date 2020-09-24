using UnityEngine;

public class BasicFiveAgentPlanet : BasePlanet
{
    public override void AditionalResets()
    {
        
    }

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
                agent.SetReward(-1f);
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
        float totalAgentAngle = 0;

        foreach (var weight in weights)
        {
            totalAngle += Vector3.Angle(weight.transform.position - Planet.transform.position, Vector3.up);
        }

        foreach (var agent in agents)
        {
            totalAgentAngle += Vector3.Angle(agent.transform.position - Planet.transform.position, Vector3.up);
        }

        totalAngle = totalAngle / weights.Count;
        totalAgentAngle = totalAgentAngle / agents.Count;

        foreach (var agent in agents)
        {
            agent.SetReward(Mathf.Pow(2,1 - totalAngle / 45));

            if (agent.IsConsuming)
            {
                agent.SetReward(Mathf.Pow(3, 1 - totalAngle / 45));
            }
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
                                weight.GetComponent<Rigidbody>().mass += 1;
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
