using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SeamType
{
    undefined = -1,
    bot = 0,
    left = 1,
    top = 2,
    right = 3,

}

[System.Serializable]
public class VerticeProperties
{
    public Color color;
    public Vector3 vector;
    public int index;
    public SeamType type;
    public bool special;
    System.Action<VerticeProperties> callback;


    public VerticeProperties(Color color, Vector3 vector, int index, System.Action<VerticeProperties> callback)
    {
        this.color = color;
        this.vector = vector;
        this.index = index;
        this.callback = callback;
        type = GetSeamType(color);
        callback.Invoke(this);
    }
    /// <summary>
    /// Optimizing this will have major impact.
    /// Currently, we do an if check on every vert, assigning a bool to later then be sent to the seam. 
    /// each property sends its bool whether or not it needs too. -fix it by adding it to a cached list then send that list to the chunk to be sent to the seams.
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public SeamType GetSeamType(Color color)
    {
        if (color.r > 0)
        {
            if (color.a < 0.9f)
                special = true;
            return SeamType.bot;
        }
        else if (color.g > 0)
        {
            if (color.a < 0.9f)
                special = true;
            return SeamType.left;
        }
        else if (color.b > 0)
        {
            if (color.a < 0.9f)
                special = true;
            return SeamType.top;
        }
        else
        {
            if (color.a < 0.9f)
                special = true;
            return SeamType.right;
        }
    }
}

[System.Serializable]
public class Seam
{
    [HideInInspector] public SeamType type;
    [HideInInspector] public VerticeProperties specialIndex;
    public List<VerticeProperties> vertProperties = new List<VerticeProperties>();
    public Seam (SeamType type)
    {
        this.type = type;
    }

    public void AddVert(VerticeProperties vert, bool isSpecial = false)
    {
        vertProperties.Add(vert);

        if (isSpecial)
            specialIndex = vert;
    }
}

[System.Serializable]
public class ChunkData
{
    public Seam bot;
    public Seam left;
    public Seam top;
    public Seam right;

    public void AssignSeams(int i, Seam seam)
    {
        if (i == 0)
            bot = seam;
        else if (i == 1)
            left = seam;
        else if (i == 2)
            top = seam;
        else if (i == 3)
            right = seam;
        else        
            Debug.Log("No Chunk Data seam list to assign to, with index of" + i);
    }

    public void UpdatePos(Transform pos)
    {
        foreach(VerticeProperties vert in bot.vertProperties)
            pos.TransformPoint(vert.vector);
    }

}

[ExecuteInEditMode]
public class Chunk : MonoBehaviour
{
    [Header("Chunk Components")]
    [SerializeField] Mesh mesh;
    [SerializeField] MeshFilter meshFilter;
    [SerializeField] MeshRenderer meshRenderer;

    [Header("Chunk Data")]
    [SerializeField] public ChunkData chunkData;

    #region private fields
    List<Seam> Seams;
    List<VerticeProperties> vertProperties;
    List<Color> testColors;
    [SerializeField] Mesh storedMesh;
    #endregion

    #region Debug
    public void SetupVertProperties()
    {
        testColors = new List<Color>();
        vertProperties = new List<VerticeProperties>();
        Seams = new List<Seam>();
        Debug.Log("Setting Up Vertex Properties");

        //Clear lists
        testColors.Clear();
        vertProperties.Clear();
        meshFilter.mesh = storedMesh;

        //Apply shared mesh to our working mesh
        mesh = WorldSpaceMesh(Instantiate(storedMesh));

        //Create a list for our vert colors to use in processing our stored vertex data to find our seams
        testColors.AddRange(mesh.colors);
        List<Color> sortedColors = new List<Color>();

        //clear and readd seam lists
        Seams.Clear();
        for (int i = 0; i < 4; i++)
            Seams.Add(new Seam((SeamType)i));


        // track mesh.vert index while sorting by colors
        int y = 0;
        foreach (Color32 color in testColors)
        {
            if (color.a != 0)            
                vertProperties.Add(new VerticeProperties(color, mesh.vertices[y], y, InitialisedVertCallback));            
            y++;
        }

        CalculateVertexData();
    }

    public void UpdatePos()
    {
        meshFilter.mesh = storedMesh;
        foreach (Seam seam in Seams)
        {
            foreach (VerticeProperties vert in seam.vertProperties)
            {
                transform.TransformPoint(vert.vector);
            }
        } 
          // transform.TransformPoint(vert);
        //CalculateVertexData();
    }

    Mesh WorldSpaceMesh(Mesh mesh)
    {
        Mesh worldSpaceMesh = mesh;

        Matrix4x4 localToWorld = transform.localToWorldMatrix;
        List<Vector3> localisedVerts = new List<Vector3>();

        for (int i = 0; i < mesh.vertices.Length; ++i)
        {
            Vector3 world_v = transform.TransformPoint(mesh.vertices[i]);
            localisedVerts.Add(world_v);
        }

        worldSpaceMesh.vertices = localisedVerts.ToArray();
        return worldSpaceMesh;

    }

    void InitialisedVertCallback(VerticeProperties properties)
    {
            Seams[SeamTypeToIndex(properties.type)].AddVert(properties, properties.special);
    }

    public int SeamTypeToIndex(SeamType type)
    {
        switch (type)
        {
            case SeamType.bot: return 0;
            case SeamType.left: return 1;
            case SeamType.top: return 2;
            case SeamType.right: return 3;

            default:
                {
                    Debug.Log("Can't find Index from Seam type");
                    break;
                }
        }
        return 0;
    }

    /// <summary>
    /// Debug!!!!!
    /// </summary>

    VerticeProperties a;
    VerticeProperties b;
    VerticeProperties c;
    VerticeProperties d;

    public void SetDrawGizmo(int i, VerticeProperties vert)
    {
        if (i == 0)
            a = vert;
        else if (i == 1)
            b = vert;
        else if (i == 2)
            c = vert;
        else if (i == 3)
            d = vert;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = a.color;
        Gizmos.DrawSphere(a.vector, 1);
        Gizmos.color = b.color;
        Gizmos.DrawSphere(b.vector, 1);
        Gizmos.color = c.color;
        Gizmos.DrawSphere(c.vector, 1);
        Gizmos.color = d.color;
        Gizmos.DrawSphere(d.vector, 1);
    }
    #endregion

    public void CalculateVertexData()
    {
        int i = 0;
        foreach (Seam seam in Seams)
        {
            SetDrawGizmo(i, seam.specialIndex);
            if (i + 1 < 4)
                Seams[i + 1].AddVert(seam.specialIndex);
            else
                Seams[0].AddVert(seam.specialIndex);

            chunkData.AssignSeams(i, seam);
            i++;
        }
    }
}
