using System.Collections.Generic;
using UnityEngine;

public abstract class BasePlanet : MonoBehaviour
{
    public List<BaseAgent> agents;
    public List<Weight> weights;
    public Transform planetPivot;
    public GamePlanet Planet;
    private List<Rigidbody> agentRBodies = new List<Rigidbody>();
    private float startTime;
    private float weightTime;
    private TensorboardObs tensorboardObs;

    // Start is called before the first frame update
    void Start()
    {
        Startup();
    }

    public void Startup()
    {
        tensorboardObs = gameObject.AddComponent<TensorboardObs>();
        // Fetch all the agent rigidbodies, assign observations
        foreach (var agent in agents)
        {
            agent.tensorboardObs = tensorboardObs;
            agentRBodies.Add(agent.GetComponent<Rigidbody>());
            foreach (var otherAgent in agents)
            {
                if (!agent.Equals(otherAgent))
                {
                    agent.AssignAgent(otherAgent.GetComponent<Rigidbody>());
                }
            }

            foreach (var weight in weights)
                agent.AssignWeight(weight);

            agent.AssignPlanet(Planet.InertialWorld);
            Planet.AddWeight(agent.rBody, agent.transform.position);
        }

        foreach (var weight in weights)
            Planet.AddWeight(weight.GetComponent<Rigidbody>(), weight.transform.position);

        //m_ResetParams = Academy.Instance.EnvironmentParameters;

        Reset();
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

        foreach (var agent in agentRBodies)
        {
            if (agent.transform.position.y < -15)
            {
                Reset();
                Debug.Log("Agent fell");
                break;
            }

            Planet.UpdateWeightPosition(agent, agent.transform.position);
        }

        foreach (var weight in weights)
            Planet.UpdateWeightPosition(weight.GetComponent<Rigidbody>(), weight.transform.position);

        if (startTime + 300 < Time.fixedTime)
        {
            Reset();
            Debug.Log("Time ran out");
        }

        float totalAngle = 0;

        foreach (var weight in weights)
        {
            totalAngle += Vector3.Angle(weight.transform.position - Planet.transform.position, Vector3.up);
        }

        foreach (var agent in agents)
        {
            totalAngle = totalAngle / weights.Count;
            agent.SetReward(1 - totalAngle / 45);

            if (agent.IsConsuming)
                agent.SetReward(1 + 1 - totalAngle / 45);
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

        tensorboardObs.ProcessOceanRewards(agents, weights, Planet);
    }

    public void Reset()
    {
        var anim = GetComponent<Animator>();

        anim.SetBool("isReset", true);

        // Disconnect the weight from the planet
        foreach (var weight in weights)
            weight.GetComponent<Joint>().connectedBody = null;

        Planet.Restart();
        Planet.transform.rotation = Quaternion.identity;

        // Call done on all the agents
        foreach (var agent in agents)
        {
            agent.EndEpisode();
            agent.CantConsume();
            agent.ResetConsumeTimer();
            agent.Reset();
        }

        // Reset all the agent positions
        foreach (var agent in agentRBodies)
        {
            agent.velocity = Vector3.zero;
            agent.angularVelocity = Vector3.zero;
            agent.transform.localPosition = new Vector3(0, 0.44f, 0.2666667f);
            agent.transform.RotateAround(planetPivot.position, planetPivot.up, Random.Range(0, 360));
        }

        // Reset the weight's position
        int numWeights = weights.Count;
        int i = 0;
        foreach (var weight in weights)
        {
            weight.transform.localPosition = new Vector3(0, 0.44f, 0.2666667f);
            weight.transform.rotation = Quaternion.Euler(30, 0, 0);
            weight.transform.RotateAround(planetPivot.position, planetPivot.up, Random.Range(0, 360));
            weight.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            weight.GetComponent<Rigidbody>().velocity = Vector3.zero;
            weight.GetComponent<Joint>().connectedBody = Planet.rigidbody;
            weight.Reset();
            if (i < numWeights)
                weight.gameObject.SetActive(true);
            else
                weight.gameObject.SetActive(false);
            i++;
        }

        startTime = Time.fixedTime;
        weightTime = Time.fixedTime;
        Invoke("ResetAnimBool", 1);
        AditionalResets();
    }

    public void ResetAnimBool()
    {
        var anim = GetComponent<Animator>();
        anim.SetBool("isReset", false);
    }

    public abstract void AditionalResets();

    public float GetStartTime()
    {
        return startTime;
    }

    public TensorboardObs GetTensorObs()
    {
        return tensorboardObs;
    }
}
