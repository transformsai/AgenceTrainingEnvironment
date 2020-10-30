using System.Collections.Generic;
using UnityEngine;

public interface IAgenceAgent : IUnityInterface
{
    bool IsConsuming { get; }
    bool IsGrounded { get; }
    int IndexInPlanet { get; }
    AgenceRewardState RewardState { get; }
}

public struct AgenceRewardState
{
    public bool DidPush;
    public bool DidConsume;
    public bool Died;
}

public interface IAgencePlanet : IUnityInterface
{
    IReadOnlyList<IAgenceAgent> Agents { get; }
    Rigidbody Rigidbody { get; }
    GameObject Lightning { get; }
    IReadOnlyList<IAgenceMcGuffin> McGuffins { get; }
}

public interface IAgenceConsumePoint : IUnityInterface
{
    bool CanBeConsumed { get; }
}

public interface IAgenceMcGuffin :IUnityInterface
{
    IReadOnlyList<IAgenceConsumePoint> ConsumePoints { get; }
}

public interface IUnityInterface
{
    Transform Transform { get;}
}


public static class InterfaceUtils
{
    public static bool Exists(this IUnityInterface obj) => obj is Object o && o;
}
