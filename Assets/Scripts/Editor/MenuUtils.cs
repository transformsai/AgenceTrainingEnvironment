using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUtils
{
    [MenuItem("Utilities/Poke Rigid Body")]
    public static void PokeRigidBody()
    {
        var obj = Selection.activeGameObject;
        if (!obj)
        {
            Debug.LogError("No object Selected");
            return;
        }

        if (!obj.scene.IsValid() || obj.scene != SceneManager.GetActiveScene())
        {
            
            Debug.LogError("Not a Scene object");
            return;
        }

        if (!Application.isPlaying)
        {
            
            Debug.LogError("Not in playmode");
            return;
        }

        var rb = obj.GetComponentInChildren<Rigidbody>();
        if (!rb)
        {
            Debug.LogError("No Rigidbody");
            return;
        }

        rb.WakeUp();
    }
}