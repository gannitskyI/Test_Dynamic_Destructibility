using System.Collections.Generic;
using UnityEngine;

public class PartMesh
{
    private readonly List<Vector3> _Vertices = new List<Vector3>();
    private readonly List<Vector3> _Normals = new List<Vector3>();
    private readonly List<List<int>> _Triangles = new List<List<int>>();
    private readonly List<Vector2> _UVs = new List<Vector2>();

    public Vector3[] Vertices;
    public Vector3[] Normals;
    public int[][] Triangles;
    public Vector2[] UV;
    public GameObject GameObject;
    public Bounds Bounds = new Bounds();

    public void AddTriangle(int submesh, Vector3 vert1, Vector3 vert2, Vector3 vert3, Vector3 normal1, Vector3 normal2, Vector3 normal3, Vector2 uv1, Vector2 uv2, Vector2 uv3)
    {
        if (_Triangles.Count - 1 < submesh)
            _Triangles.Add(new List<int>());

        _Triangles[submesh].AddRange(new[] { _Vertices.Count, _Vertices.Count + 1, _Vertices.Count + 2 });
        _Vertices.AddRange(new[] { vert1, vert2, vert3 });
        _Normals.AddRange(new[] { normal1, normal2, normal3 });
        _UVs.AddRange(new[] { uv1, uv2, uv3 });

        Bounds.Encapsulate(vert1);
        Bounds.Encapsulate(vert2);
        Bounds.Encapsulate(vert3);
    }

    public void FillArrays()
    {
        Vertices = _Vertices.ToArray();
        Normals = _Normals.ToArray();
        UV = _UVs.ToArray();
        Triangles = new int[_Triangles.Count][];
        for (int i = 0; i < _Triangles.Count; i++)
            Triangles[i] = _Triangles[i].ToArray();
    }

    public void MakeGameObject(MeshDestroy original, Material[] materials)
    {
        GameObject = new GameObject(original.name);
        GameObject.transform.position = original.transform.position;
        GameObject.transform.rotation = original.transform.rotation;
        GameObject.transform.localScale = original.transform.localScale;

        Mesh mesh = new Mesh
        {
            name = original.GetComponent<MeshFilter>().mesh.name,
            vertices = Vertices,
            normals = Normals,
            uv = UV
        };

        for (int i = 0; i < Triangles.Length; i++)
            mesh.SetTriangles(Triangles[i], i, true);
        mesh.RecalculateBounds();

        GameObject.AddComponent<MeshRenderer>().materials = materials;
        GameObject.AddComponent<MeshFilter>().mesh = mesh;
        GameObject.AddComponent<MeshCollider>().convex = true;

        Rigidbody rb = GameObject.AddComponent<Rigidbody>();
        rb.mass = 1.0f;
        rb.drag = 0.5f;
        rb.angularDrag = 0.5f;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
    }
}
