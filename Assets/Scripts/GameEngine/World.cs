using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class World : MonoBehaviour, IAgencePlanet
{


    public Transform Transform => transform;
    IReadOnlyList<IAgenceAgent> IAgencePlanet.Agents => RLAgents != null ? RLAgents :
        RLAgents = Agents.Select(it => it.GetComponent<AgenceRLAgent>()).ToList();
    IReadOnlyList<IAgenceMcGuffin> IAgencePlanet.McGuffins => McGuffins;
    private List<AgenceRLAgent> RLAgents;
    public GameObject Lightning => null; //Todo: simulate lightning movement.
    public Rigidbody Rigidbody => Planet.rigidbody;


    [Header("Scene References")]

    public Planet Planet;
    public List<AgentController> Agents = new List<AgentController>();
    public List<McGuffin> McGuffins = new List<McGuffin>();

    //Todo: public StoryManager StoryManager;

    public Vector3 Centre => Planet.transform.position;
    
    private void OnDestroy() => ClearWorld();

    public void ClearWorld()
    {
        foreach (var agent in Agents) if (agent) Destroy(agent.gameObject);
        foreach (var mcGuffin in McGuffins) if (mcGuffin) Destroy(mcGuffin.gameObject);

        Agents.Clear();
        McGuffins.Clear();
    }

    public int AgentsRemaining()
    {
        var count = 0;
        foreach (var agent in Agents)
        {
            if (agent) ++count;
        }

        return count;
    }


    private McGuffin _largestMcg = null;
    private int _largestMcgFrameCount = 0;
    public Transform AgentBonesTransform;

    public McGuffin LargestMcg
    {
        get
        {
            if (Time.frameCount == _largestMcgFrameCount) return _largestMcg;

            _largestMcg = null;
            foreach (var mcg in McGuffins)
            {
                if (_largestMcg == null) _largestMcg = mcg;
                if (!mcg) continue;
                if (mcg.Size < _largestMcg.Size) continue;
                if (mcg.Size == _largestMcg.Size && mcg.transform.position.y < _largestMcg.transform.position.y) continue;
                _largestMcg = mcg;
            }

            _largestMcgFrameCount = Time.frameCount;
            return _largestMcg;
        }
    }


    public Vector3 ToSurfacePoint(Vector3 equatorPoint, bool isRelative)
    {
        if (!isRelative) equatorPoint -= Centre;
        equatorPoint.y = 0;
        equatorPoint = Vector3.ClampMagnitude(equatorPoint, Planet.Radius);

        //Pythagorean theorem
        var c = Planet.Radius;
        var a = equatorPoint.magnitude;
        var b = Mathf.Sqrt(c * c - a * a);

        return Centre + equatorPoint + Vector3.up * b;

    }

    public float GetAngle(Transform target) => target ? Vector3.Angle(target.position - Centre, Vector3.up) : 0;

    public ConsumePoint GetClosestConsumePoint(Vector3 worldPos)
    {
        var minDist = float.MaxValue;
        var cp = default(ConsumePoint);

        foreach (var mcGuffin in McGuffins)
        {
            if(!mcGuffin) continue;
            
            foreach (var consumePoint in mcGuffin.consumePoints)
            {
                if(!consumePoint) continue;
                var dist = (consumePoint.transform.position - worldPos).sqrMagnitude;
                if (dist < minDist) cp = consumePoint;
            }
        }

        return cp;
    }

}

public enum WorldState
{
    NotStarted,
    Simulating,
    Completed
}