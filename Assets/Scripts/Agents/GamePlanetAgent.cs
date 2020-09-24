using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine.InputSystem;

public class GamePlanetAgent : BaseAgent
{
    public StrippedAgentController controller;
    public float rlMovementMagnitude;

    public override void OnActionReceived(float[] vectorAction)
    {
        Vector3 movement = Vector3.zero;
        movement.x = vectorAction[0];
        movement.z = vectorAction[1];

        var push = vectorAction[2];

        if (push > 0)
            controller.Push();

        movement = movement * 0.5f;
        Quaternion rotation = Quaternion.Euler(movement);
        var relpos = transform.position - GetPlanet().transform.position;
        var targetPos = rotation * relpos;
        var absPos = targetPos + GetPlanet().transform.position;

        var relPoint = WorldToRelativePoint(absPos) * rlMovementMagnitude;
        controller.Stamina = 1;
        
        controller.UpdateMotion(relPoint);
    }

    public Vector2 WorldToRelativePoint(Vector3 pt)
    {
        var position = transform.position;
        var invVec = transform.InverseTransformVector(pt - position);
        return new Vector2(invVec.x, invVec.z);
    }
}
