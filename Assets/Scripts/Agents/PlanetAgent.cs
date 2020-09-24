using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetAgent : Agent
{
    public Rigidbody weight;
    public Joint joint;
    public Transform planetPivot;
    public List<Rigidbody> agents;
    private Rigidbody rBody;
    private float startTime;
    // Start is called before the first frame update
    void Start()
    {
        // Get the planets rigidbody
        rBody = this.GetComponent<Rigidbody>();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        foreach(var agent in agents)
        {
            sensor.AddObservation(agent.transform.position);
            sensor.AddObservation(agent.velocity);
        }

        sensor.AddObservation(weight.transform.position);
        sensor.AddObservation(weight.velocity);

        sensor.AddObservation(this.transform.rotation.eulerAngles);
        sensor.AddObservation(rBody.velocity);
    }

    public override void OnEpisodeBegin()
    {
        // Disconnect the weight from the planet
        joint.connectedBody = null;

        rBody.transform.rotation = Quaternion.Euler(0, 0, 0);

        // Reset all the agent positions
        foreach (var agent in agents)
        {
            agent.velocity = Vector3.zero;
            agent.angularVelocity = Vector3.zero;
            agent.transform.position = new Vector3(0, -1.8f, 8);
            agent.transform.RotateAround(planetPivot.position, planetPivot.up, Random.Range(0, 360));
        }

        // Reset the weight's position
        weight.transform.position = new Vector3(0, -1.8f, 8);
        weight.transform.RotateAround(planetPivot.position, planetPivot.up, Random.Range(0, 360));
        weight.angularVelocity = Vector3.zero;
        weight.velocity = Vector3.zero;

        // Reconnect the weight
        joint.connectedBody = rBody;

        // Reset the planet
        rBody.angularVelocity = Vector3.zero;
        rBody.velocity = Vector3.zero;

        startTime = Time.time;
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        Vector3 movement1 = Vector3.zero;
        movement1.x = vectorAction[0];
        movement1.z = vectorAction[1];

        movement1 = movement1 * 0.5f;
        Quaternion rotation1 = Quaternion.Euler(movement1);

        agents[0].transform.localPosition = rotation1 * agents[0].transform.localPosition;

        Vector3 movement2 = Vector3.zero;
        movement2.x = vectorAction[2];
        movement2.z = vectorAction[3];

        movement2 = movement2 * 0.5f;
        Quaternion rotation2 = Quaternion.Euler(movement2);

        agents[1].transform.localPosition = rotation2 * agents[1].transform.localPosition;


        if (weight.transform.position.y < -15)
        {
            EndEpisode();
            Debug.Log("Weight Fell");
        }

        foreach (var agent in agents)
        {
            if (agent.transform.position.y < -15)
            {
                EndEpisode();
                Debug.Log("Agent fell");
                break;
            }
        }

        if (startTime + 60 < Time.time)
        {
            EndEpisode();
            Debug.Log("Time ran out");
        }
        SetReward(0.1f);
    }

    private void FixedUpdate()
    {
        foreach (var agent in agents)
        {
            rBody.AddForceAtPosition(new Vector3(0, -1 * 9.8f, 0), agent.transform.position);
        }
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = -Input.GetAxis("Horizontal");
        actionsOut[1] = Input.GetAxis("Vertical");
        actionsOut[2] = -Input.GetAxis("Horizontal");
        actionsOut[3] = Input.GetAxis("Vertical");
    }
}
