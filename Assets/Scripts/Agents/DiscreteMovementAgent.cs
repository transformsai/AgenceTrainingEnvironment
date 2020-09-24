using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class DiscreteMovementAgent : BaseAgent
{
    public override void OnActionReceived(float[] vectorAction)
    {
        Vector3 movement = Vector3.zero;
        int forward = (int)vectorAction[0];
        int backward = (int)vectorAction[1];
        int right = (int)vectorAction[2];
        int left = (int)vectorAction[3];
        int distance = (int)vectorAction[4];

        int push = (int)vectorAction[5];

        if(forward == 1)
        {
            if (distance == 1)
                movement.x = 1;
            else
                movement.x = 0.5f;
        }
        else if (backward == 1)
        {
            if (distance == 1)
                movement.x = -1;
            else
                movement.x = -0.5f;
        }
        if (right == 1)
        {
            if (distance == 1)
                movement.z = -1;
            else
                movement.z = -0.5f;
        }
        else if (left == 1)
        {
            if (distance == 1)
                movement.z = 1;
            else
                movement.z = 0.5f;
        }

        if (push == 1 && pushTarget)
            pushTarget.Push();


        rewardDisplay = GetCumulativeReward();

        movement = movement * 0.5f;
        Quaternion rotation = Quaternion.Euler(movement);

        if (!IsConsuming && Time.fixedTime > GetPushTime())
            this.transform.localPosition = rotation * this.transform.localPosition;
    }
}
