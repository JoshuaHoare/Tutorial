using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VerticeProperties
{
    public Color32 color;
    public Vector3 vector;
    public int index;

    public VerticeProperties(Color32 color, Vector3 vector, int index)
    {
        this.color = color;
        this.vector = vector;
        this.index = index;
    }
}

[ExecuteInEditMode]
public class Chunk : MonoBehaviour
{
    [Header("Chunk Components")]
    [SerializeField] Mesh mesh;
    [SerializeField] MeshFilter meshFilter;
    [SerializeField] MeshRenderer meshRenderer;

    [Header("Seams")]
    [SerializeField] List<List<Vector3>> Seams;
    [SerializeField] List<VerticeProperties> vertProperties;
    [SerializeField] List<Vector3> testSeam;
    [SerializeField] List<Color32> testColors;
    [SerializeField] List<Vector3> sortedVerts;

    #region Debug
    public void SetupVertProperties()
    {
        Debug.Log("Setting Up Vertex Properties");
        testColors.Clear();
        mesh = meshFilter.sharedMesh;
        testColors.AddRange(mesh.colors32);
        List<Color32> sortedColors = new List<Color32>();
        float alphaCase = testColors[4].a;
        int i = 0;
        foreach (Color32 color in testColors)
        {
            if (color.a != 0)
            {
                vertProperties.Add(new VerticeProperties(color, mesh.vertices[i], i));
            }
            i++;
        }
    }
    #endregion

    public void CalculateVertexData()
    {

    }
}
