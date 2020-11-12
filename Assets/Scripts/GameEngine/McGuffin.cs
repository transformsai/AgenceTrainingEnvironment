using System.Collections.Generic;
using UnityEngine;

public class McGuffin : MonoBehaviour, IAgenceMcGuffin
{

    private static readonly int AnimActive = Animator.StringToHash("Consumepoint1Active");
    private static readonly int AnimConsumepoint2Active1 = Animator.StringToHash("Consumepoint2Active");
    private static readonly int AnimConsumepoint3Active1 = Animator.StringToHash("Consumepoint3Active");

    public Transform Transform => transform;
    public SettingsContainer Settings => GameManager.Settings;
    public IReadOnlyList<IAgenceConsumePoint> ConsumePoints => consumePoints;

    public List<ConsumePoint> consumePoints;

    private Rigidbody rbody;
    private Animator animator;

    public World World;
    public Joint Joint;

    public float Size = 0;
    public float SizeGainMultiplier = 1;

    private void Awake()
    {
        
        animator = GetComponentInChildren<Animator>();
        rbody = GetComponent<Rigidbody>();
        World = GetComponentInParent<World>();
        Joint.connectedBody = World.Planet.InertialWorld;
        World.McGuffins.Add(this);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var cp1Active = !consumePoints[0].CanBeConsumed;
        var cp2Active = !consumePoints[1].CanBeConsumed;
        var cp3Active = !consumePoints[2].CanBeConsumed;

        animator.SetBool(AnimActive, cp1Active);
        animator.SetBool(AnimConsumepoint2Active1, cp2Active);
        animator.SetBool(AnimConsumepoint3Active1, cp3Active);

        if (cp1Active || cp2Active || cp3Active)
        {
            Size += SizeGainMultiplier * Settings.sizeGainPerSecondConsumed;
        }

        Size += SizeGainMultiplier * Settings.autoSizeGainPerSecond;

    }


    public void Reset()
    {
        if (rbody) rbody.mass = Mathf.Lerp(Settings.minMcGuffinMass, Settings.maxMcGuffinMass, Size);
    }
}
