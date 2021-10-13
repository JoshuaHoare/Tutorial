using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Chunk))]
public class ChunkCustomInspector : Editor
{
    bool setup = false;
    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("3D Model Properties");
        DrawDefaultInspector();

        Chunk chunk = (Chunk)target;
        if (GUILayout.Button("Setup Vert Properties"))
        {
            setup = true;
            chunk.SetupVertProperties();
        }
        if (GUILayout.Button("Reload") && setup)
            chunk.CalculateVertexData();
    }
}
