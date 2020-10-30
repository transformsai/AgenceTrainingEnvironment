using UnityEngine;

public class HoldingPlanetState : AgentState
{
    private Planet Planet => Controller.World.Planet;
    private readonly CharacterJoint _holdTarget; // Where on the planet to hold
    private readonly Rigidbody _planetRb;
    private float enterTime;

    public HoldingPlanetState(AgentController controller) : base(controller)
    {
        _planetRb = Planet.GetComponent<Rigidbody>();
        _holdTarget = CreateReachAnchor();
    }

    public override bool CanEnter
    {
        get
        {
            if (Controller.CurrentState == this) return false;
            if (!Controller.IsGrounded) return false;
            if (Controller.Stamina <= Settings.minHoldStamina) return false;
            // Ensure reach target is correctly positioned before entering
            
            return true;
        }
    }


    public override void Enter()
    {
        var pos = Transform.position;
        var rayDir = Controller.World.Centre - pos ;
        
        Controller.World.Planet.collider.Raycast(new Ray(pos - rayDir, rayDir * 2), out var hit, rayDir.magnitude * 2);

        _holdTarget.transform.position = hit.point;
        


        // Hold on to planet at reach target
        var targetPos = _holdTarget.transform.position;
        //controller.JointController.EnableJoint(Transform.InverseTransformPoint(targetPos), _planetRb);
        Planet.AddWeight(Rigidbody, targetPos);
        Rigidbody.isKinematic = true;
        Rigidbody.position = targetPos;
        Rigidbody.transform.up = (targetPos - Planet.transform.position).normalized;
        Rigidbody.useGravity = true;;
        
        
    }

    public override void Stay()
    {

        Rigidbody.transform.position = _holdTarget.transform.position;
        Rigidbody.transform.up = (_holdTarget.transform.position - Planet.transform.position).normalized;
        Rigidbody.isKinematic = true;

        Controller.Stamina -= Time.fixedDeltaTime * Settings.holdStaminaCost;
        Planet.UpdateWeightPosition(Rigidbody, _holdTarget.transform.position);

        bool canHold = 0 < Controller.Stamina && Controller.Control.HoldGround;
        if (!canHold && (Time.time - enterTime) > Settings.minHoldDuration) Controller.TryTransition<MovingState>();
    }

    public override void Exit()
    {
        // Stop holding
        //controller.JointController.DisableJoint();
        Planet.RemoveWeight(Rigidbody);
        Rigidbody.isKinematic = false;
        Rigidbody.useGravity = true;


    }

    private CharacterJoint CreateReachAnchor()
    {
        GameObject go = new GameObject(Controller.gameObject.name + " Hold Target");
        var transform = go.transform;
        transform.SetParent(_planetRb.transform, true);
        var rb = go.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        var joint =  go.AddComponent<CharacterJoint>();
        joint.enableCollision = true;
        joint.autoConfigureConnectedAnchor = false;
        joint.enablePreprocessing = false;

        
        return joint;

    }

}
