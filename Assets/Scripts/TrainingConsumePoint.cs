using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TrainingConsumePoint : MonoBehaviour
{
    public bool CanConsume;
    private BaseAgent agent = null;
    private float ConsumeTimer;
    private float RechargeTimer;
    // Start is called before the first frame update
    void Start()
    {
        CanConsume = true;
    }

    public void OnConsumption(BaseAgent consumingAgent)
    {
        Debug.Log("Agent Consuming");
        agent = consumingAgent;
        agent.Consume(this);
        CanConsume = false;
        ConsumeTimer = Time.fixedTime + 8;
        RechargeTimer = Time.fixedTime + 16;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.fixedTime > ConsumeTimer && agent)
        {
            Debug.Log("Agent Exit Consumption");
            agent.IsConsuming = false;
            agent = null;
        }
        if(Time.fixedTime > RechargeTimer)
        {
            CanConsume = true;
        }
    }
}
