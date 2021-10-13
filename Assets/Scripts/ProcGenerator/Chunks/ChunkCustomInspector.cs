using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Chunk))]
public class ChunkCustomInspector : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("3D Model Properties");
        DrawDefaultInspector();

        Chunk chunk = (Chunk)target;
        if (GUILayout.Button("Reload"))
        {
            chunk.SetupVertProperties();
        }

    }
}
