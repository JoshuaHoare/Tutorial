using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class Vertex
{
    public int index;
    public Color32 color;
    public Vector3 position;
    public int triangles;

    public Vertex(int index, Vector3 position, Color32 color, int triangles)
    {
        this.index = index;
        this.color = color;
        this.position = position;
        this.triangles = triangles;
    }
}

public class ArtFile : MonoBehaviour
{
    [Header("Debugging")]
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] MeshFilter meshFilter;
    [SerializeField] Mesh mesh;

    [SerializeField] List<Vertex> vertices;
    [SerializeField] List<Vertex> dynamicVerts;

    [Header("Size")]
    [SerializeField][Range(0,1)] float size;

    public void Yeet()
    {
        mesh = meshFilter.sharedMesh;

        vertices = new List<Vertex>();
        dynamicVerts = new List<Vertex>();

        for (int i = 0; i < mesh.vertices.Length; i++)
            vertices.Add(new Vertex(i, mesh.vertices[i], mesh.colors[i], mesh.triangles[i]));        
        
        foreach (Vertex vert in vertices)
        {
            if (vert.color.a == 0)
                dynamicVerts.Add(vert);                
        }
    }

    void UpdateMesh()
    {
        mesh.Clear();

        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Color32> colors = new List<Color32>();

        foreach (Vertex vert in vertices)
        {
            verts.Add(vert.position);
            tris.Add(vert.triangles);
            colors.Add(vert.color);
        }

        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.colors32 = colors.ToArray();

        mesh.RecalculateNormals();
    }

    public void IncreaseSize()
    {
        foreach (Vertex vert in dynamicVerts)
        {
            vert.position.x -= size;
        }

        UpdateMesh();
    }
}
