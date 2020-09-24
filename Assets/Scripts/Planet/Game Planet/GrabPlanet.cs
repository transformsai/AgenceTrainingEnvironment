using Unity.MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JetBrains.Annotations;

public class GrabPlanet : BasePlanet
{
    public GameObject lightning;
    private float lightningTime;
    // Update is called once per frame

    void Start()
    {
        lightningTime = Time.fixedTime;
        Startup();
    }

    public override void AditionalResets()
    {
        lightningTime = Time.fixedTime;
    }

    void FixedUpdate()
    {
        if((Time.fixedTime - lightningTime) % 20 == 0)
        {
            foreach(var agent in agents)
            {
                if(Vector3.Distance(agent.transform.position, lightning.transform.position) <= 2.5f)
                {
                    Debug.Log("lightning strike");
                    agent.SetReward(-1f);
                    Reset();
                }
                agent.SetLightningPos(null);
            }
        }
        else if((Time.fixedTime - lightningTime) % 10 == 0)
        {
            lightning.transform.localPosition = new Vector3(0, 0.44f, 0.2666667f);
            lightning.transform.rotation = Quaternion.Euler(30, 0, 0);
            lightning.transform.RotateAround(planetPivot.position, transform.up, Random.Range(0, 360));

            foreach (var agent in agents)
            {
                agent.SetLightningPos(lightning);
            }
        }

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
            agent.SetReward((1 - totalAngle / 45) * agent.health);

            if (Vector3.Distance(agent.transform.position, lightning.transform.position) <= 2.5f)
            {
                agent.SetReward(-0.5f);
                Debug.Log("agent in lightning");
            }
            else if (agent.IsConsuming)
            {
                agent.SetReward((1 + 1 - totalAngle / 45) * agent.health);
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
                                // weight.GetComponent<Rigidbody>().mass += 1;
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
