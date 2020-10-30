using UnityEngine;

public class ReadOnlyAttribute : PropertyAttribute
{
 
}



#if UNITY_EDITOR
[UnityEditor.CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : UnityEditor.PropertyDrawer
{
    public override float GetPropertyHeight(UnityEditor.SerializedProperty property,
        GUIContent label)
    {
        return UnityEditor.EditorGUI.GetPropertyHeight(property, label, true);
    }
 
    public override void OnGUI(Rect position,
        UnityEditor.SerializedProperty property,
        GUIContent label)
    {
        GUI.enabled = false;
        UnityEditor.EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}
#endif
