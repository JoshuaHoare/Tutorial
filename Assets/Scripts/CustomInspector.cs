using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ArtFile))]
public class CustomInspector : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("3D Model Properties");
        DrawDefaultInspector();

        ArtFile Component = (ArtFile)target;
        if (GUILayout.Button("Editor Update"))
            Component.Yeet();

        if (GUILayout.Button("Increase Size"))
            Component.IncreaseSize();
    }
}
