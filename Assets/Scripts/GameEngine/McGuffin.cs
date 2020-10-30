using System;
using System.Collections.Generic;
using UnityEngine;

public class McGuffin : MonoBehaviour, IAgenceMcGuffin
{
    public Transform Transform => transform;
    public IReadOnlyList<IAgenceConsumePoint> ConsumePoints => consumePoints;

    public bool Consumepoint1Active = true;
    public bool Consumepoint2Active = true;
    public bool Consumepoint3Active = true;

    public List<ConsumePoint> consumePoints;
    public float minMass;
    public float maxMass;
    public int fixedUpdateLength;
    private Rigidbody rbody;
    private Animator animator;
    public float Size;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        rbody = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Consumepoint1Active = !consumePoints[0].CanBeConsumed;
        Consumepoint2Active = !consumePoints[1].CanBeConsumed;
        Consumepoint3Active = !consumePoints[2].CanBeConsumed;

        animator.SetBool("Consumepoint1Active", Consumepoint1Active);
        animator.SetBool("Consumepoint2Active", Consumepoint2Active);
        animator.SetBool("Consumepoint3Active", Consumepoint3Active);
    }


    public void Reset()
    {
        if(rbody)
            rbody.mass = minMass;
    }
}
