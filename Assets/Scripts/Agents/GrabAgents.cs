using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabAgents : BaseAgent
{
    public Joint planetJoint;
    public override void OnActionReceived(float[] vectorAction)
    {
        Vector3 movement = Vector3.zero;
        movement.x = vectorAction[0];
        movement.z = vectorAction[1];

        var push = vectorAction[2];

        if (push > 0)
        {
            tensorboardObs.UpdateStatsRecorder(this.name + " Push Action", 1);
        }
        else
        {
            tensorboardObs.UpdateStatsRecorder(this.name + " Push Action", 0);
            planetJoint.connectedBody = null;
        }

        if (push > 0 && pushTarget && !pushTarget.IsStunned && Time.fixedTime > GetPushTime() && Time.fixedTime > GetPushActionTime())
        {
            SetPushActionTime(Time.fixedTime + 2);
            pushTarget.Push();
        }
        else if(push > 0 && !pushTarget)
        {
            planetJoint.connectedBody = GetPlanet();
            SetHolding(true);
        }
        else
        {
            SetHolding(false);
        }

        rewardDisplay = GetCumulativeReward();

        movement = movement * 0.5f;
        Quaternion rotation = Quaternion.Euler(movement);

        if (!IsConsuming && Time.fixedTime > GetPushTime() && Time.fixedTime > GetPushActionTime() && !GetHolding())
        {
            IsStunned = false;
            this.transform.localPosition = rotation * this.transform.localPosition;
        }

        if (!(health >= 1) && IsConsuming)
            health += 0.005f;
        else if (!(health >= 1))
            health += 0.001f;
    }
}
