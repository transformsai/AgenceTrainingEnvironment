using UnityEngine;

public class ConsumePoint : MonoBehaviour, IAgenceConsumePoint
{
    public McGuffin McGuffin;
    private AgentController agent = null;
    private float RechargeTime;
    public Transform Transform => transform;

    public bool CanBeConsumed => !agent && Time.fixedTime > RechargeTime;

    public bool TryUsePoint(ConsumingState consumingState)
    {
        if (!CanBeConsumed) return false;
        agent = consumingState.Controller;
        return true;
    }

    public void ConsumeFinished()
    {
        RechargeTime = Time.time + 8;
        agent = null;
    }

}
