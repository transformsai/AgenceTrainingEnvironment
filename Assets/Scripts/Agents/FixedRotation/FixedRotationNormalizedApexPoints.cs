using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class FixedRotationNormalizedApexPoints : Agent
{
    public float rewardDisplay = 0;
    public Rigidbody rBody;
    private Rigidbody planet;
    private Rigidbody weight;
    private List<Rigidbody> agents = new List<Rigidbody>();
    private Dictionary<string, WelfordVariance> variances = new Dictionary<string, WelfordVariance>();

    [TextArea(minLines: 12, maxLines: 24)]
    public string obsDisplayText = "";

    void Start()
    {
        //rBody = GetComponent<Rigidbody>();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if (planet && rBody && weight)
        {
            obsDisplayText = "";
            LogVectorObs("position", planet.transform.rotation * this.transform.localPosition, new Vector3(-0.5f, 0, -0.5f), new Vector3(0.5f, 0.6f, 0.5f), sensor);
            LogVectorObs("Planet angular velocity", planet.angularVelocity, new Vector3(-3, -3, -3), new Vector3(3, 3, 3), sensor);
            LogVectorObs("Weight position", planet.transform.rotation * weight.transform.localPosition, new Vector3(-0.5f, 0, -0.5f), new Vector3(0.5f, 0.6f, 0.5f), sensor);
            LogVectorObs("Self Tangential Velocity", planet.GetPointVelocity(this.transform.position), new Vector3(-3, -3, -3), new Vector3(3, 3, 3), sensor);
            LogVectorObs("Weight Tangential Velocity", planet.GetPointVelocity(weight.transform.position), new Vector3(-3, -3, -3), new Vector3(3, 3, 3), sensor);

            int agentnum = 0;
            foreach (var agent in agents)
            {
                LogVectorObs("position", planet.transform.rotation * agent.transform.localPosition, new Vector3(-0.5f, 0, -0.5f), new Vector3(0.5f, 0.6f, 0.5f), sensor);
                LogVectorObs("Other Agent's Tangential Velocity", planet.GetPointVelocity(agent.transform.position), new Vector3(-3, -3, -3), new Vector3(3, 3, 3), sensor);
                agentnum++;
            }

            if (agents.Count == 0)
            {
                LogVectorObs("position", Vector3.zero, new Vector3(-0.5f, 0, -0.5f), new Vector3(0.5f, 0.6f, 0.5f), sensor);
                LogVectorObs("Other Agent's Tangential Velocity", Vector3.zero, new Vector3(-3, -3, -3), new Vector3(3, 3, 3), sensor);
            }
        }
    }

    public void LogVectorObs(string obsName, Vector3 obsVector, Vector3 min, Vector3 max, VectorSensor sensor)
    {
        if (!variances.ContainsKey(obsName))
        {
            variances.Add(obsName, new WelfordVariance());
        }
        variances[obsName].UpdateVariance(new Quaternion(obsVector.x, obsVector.y, obsVector.z, 0));

        var normalizedObs = new Vector3(((obsVector.x - min.x) / (max.x - min.x)),
            ((obsVector.y - min.y) / (max.y - min.y)),
            ((obsVector.z - min.z) / (max.z - min.z)));
        sensor.AddObservation(normalizedObs);

        if (Application.isEditor)
        {
            obsDisplayText += string.Format("\n{0:0.00} {1} \n{2:0.00} \n{3:0.00}", obsVector.x, obsName, obsVector.y, obsVector.z);
            obsDisplayText += string.Format("\n{0:0.00} {1} \n{2:0.00} \n{3:0.00}", normalizedObs.x, obsName + " normalized", normalizedObs.y, normalizedObs.z);
        }
        
    }

    public void LogVectorObs(string obsName, Quaternion obsVector, VectorSensor sensor)
    {
        if (Application.isEditor)
        {
            obsDisplayText += string.Format("\n{0:0.00} {1} \n{2:0.00} \n{3:0.00} \n{4:0.00}", obsVector.x, obsName, obsVector.y, obsVector.z, obsVector.w);
        }
        sensor.AddObservation(obsVector);
    }

    public void LogVectorObs(string obsName, float obsVector, VectorSensor sensor)
    {
        if (Application.isEditor)
        {
            obsDisplayText += string.Format("\n{0:0.00} {1}", obsVector, obsName);
        }
        sensor.AddObservation(obsVector);
    }

    public void AssignPlanet(Rigidbody planet)
    {
        this.planet = planet;
    }

    public void AssignAgent(Rigidbody agent)
    {
        this.agents.Add(agent);
    }

    public void AssignWeight(Rigidbody weight)
    {
        this.weight = weight;
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        Vector3 movement = Vector3.zero;
        movement.x = vectorAction[0];
        movement.z = vectorAction[1];

        rewardDisplay = GetCumulativeReward();

        movement = movement * 0.5f;
        Quaternion rotation = Quaternion.Euler(movement);

        this.transform.localPosition = rotation * this.transform.localPosition;

        rewardDisplay = GetCumulativeReward();
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = -Input.GetAxis("Horizontal");
        actionsOut[1] = Input.GetAxis("Vertical");
    }

    private class WelfordVariance
    {
        int count;
        Quaternion mean;
        Quaternion m2;
        Quaternion? min;
        Quaternion? max;

        public WelfordVariance()
        {
            count = 0;
            mean = Quaternion.identity;
            m2 = Quaternion.identity;
            min = null;
            max = null;
        }

        public void UpdateVariance(Quaternion newValue)
        {
            if (!max.HasValue)
                max = newValue;
            else
            {
                if (newValue.x > max.Value.x)
                    max = new Quaternion(newValue.x, max.Value.y, max.Value.z, max.Value.w);
                if (newValue.y > max.Value.y)
                    max = new Quaternion(max.Value.x, newValue.y, max.Value.z, max.Value.w);
                if (newValue.z > max.Value.z)
                    max = new Quaternion(max.Value.x, max.Value.y, newValue.z, max.Value.w);
                if (newValue.w > max.Value.w)
                    max = new Quaternion(max.Value.x, max.Value.y, max.Value.z, newValue.w);
            }

            if (!min.HasValue)
                min = newValue;
            else
            {
                if (newValue.x < min.Value.x)
                    min = new Quaternion(newValue.x, min.Value.y, min.Value.z, min.Value.w);
                if (newValue.y < min.Value.y)
                    min = new Quaternion(min.Value.x, newValue.y, min.Value.z, min.Value.w);
                if (newValue.z < min.Value.z)
                    min = new Quaternion(min.Value.x, min.Value.y, newValue.z, min.Value.w);
                if (newValue.w < min.Value.w)
                    min = new Quaternion(min.Value.x, min.Value.y, min.Value.z, newValue.w);
            }

            count += 1;

            Quaternion delta = Quaternion.identity;
            delta.x = newValue.x - mean.x;
            delta.y = newValue.y - mean.y;
            delta.z = newValue.z - mean.z;
            delta.w = newValue.w - mean.w;

            mean.x += (delta.x - mean.x) / count;
            mean.y += (delta.y - mean.y) / count;
            mean.z += (delta.z - mean.z) / count;
            mean.w += (delta.w - mean.w) / count;

            Quaternion delta2 = Quaternion.identity;
            delta2.x = newValue.x - mean.x;
            delta2.y = newValue.y - mean.y;
            delta2.z = newValue.z - mean.z;
            delta2.w = newValue.w - mean.w;

            m2.x += delta.x * delta2.x;
            m2.y += delta.y * delta2.y;
            m2.z += delta.z * delta2.z;
            m2.w += delta.w * delta2.w;
        }

        public Quaternion? GetVariance()
        {
            if (count < 2)
                return null;
            else
            {
                Quaternion returnValue = Quaternion.identity;
                returnValue.x = m2.x / count;
                returnValue.y = m2.y / count;
                returnValue.z = m2.z / count;
                returnValue.w = m2.w / count;
                return returnValue;
            }
        }

        public Quaternion? GetMean()
        {
            if (count < 2)
                return null;
            else
                return mean;
        }

        public Quaternion? GetMin()
        {
            return min;
        }

        public Quaternion? GetMax()
        {
            return max;
        }
    }
}
