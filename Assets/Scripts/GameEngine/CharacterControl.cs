using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public struct CharacterControl
{
    public Vector2 RelativeMovement; // +y = forward, +x = right; magnitude is 0-1
    public bool Consume;
    public bool Headbutt;
    public bool Sit;
    public bool HoldGround;
}
public enum ControlSources
{
    ScriptedBehaviour = 0,
    MachineLearning = 1,
    UserControls = 2,
}
public class ControlSourceHandler
{
    private const float ControlExpiryOffset = 3;


    private static readonly ControlSources[] AllSources =
        Enum.GetValues(typeof(ControlSources))
            .Cast<ControlSources>()
            .OrderByDescending(it => (int)it)
            .ToArray();


    private Dictionary<ControlSources, float> SubmitTimes = new Dictionary<ControlSources, float>();
    private Dictionary<ControlSources, CharacterControl> Controls = new Dictionary<ControlSources, CharacterControl>();

    public CharacterControl GetControl()
    {



        foreach (var source in AllSources)
        {
            var hasValue = SubmitTimes.TryGetValue(source, out var submitTime);
            if (hasValue && submitTime + ControlExpiryOffset > Time.fixedTime) 
                return Controls[source];
        }
        return new CharacterControl();
    }

    public void SubmitControls(CharacterControl control, ControlSources controlSource)
    {
        SubmitTimes[controlSource] = Time.fixedTime;
        Controls[controlSource] = control;
    }
}