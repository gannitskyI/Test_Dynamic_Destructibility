using System.Collections.Generic;
using UnityEngine;

public class MeshDestroy : MonoBehaviour
{  
    [SerializeField] private int CutCascades = 1;
    [SerializeField] private float ExplodeForce = 0;

    private Mesh originalMesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Vector3[] originalVertices;
    private Vector3[] originalNormals;
    private Vector2[] originalUVs;
    private int[][] originalTriangles;

    private IMeshCutter meshCutter;
    private IForceApplier forceApplier;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        originalMesh = meshFilter.mesh;
        CacheOriginalMeshData();

        meshCutter = new PlaneMeshCutter();
        forceApplier = new ExplosiveForceApplier(ExplodeForce);
    }

    private void CacheOriginalMeshData()
    {
        originalMesh.RecalculateBounds();
        originalVertices = originalMesh.vertices;
        originalNormals = originalMesh.normals;
        originalUVs = originalMesh.uv;
        originalTriangles = new int[originalMesh.subMeshCount][];
        for (int i = 0; i < originalMesh.subMeshCount; i++)
        {
            originalTriangles[i] = originalMesh.GetTriangles(i);
        }
    }

    private void DestroyMesh()
    {
        List<PartMesh> parts = new List<PartMesh>
        {
            new PartMesh
            {
                UV = originalUVs,
                Vertices = originalVertices,
                Normals = originalNormals,
                Triangles = originalTriangles,
                Bounds = originalMesh.bounds
            }
        };

        for (int c = 0; c < CutCascades; c++)
        {
            List<PartMesh> subParts = new List<PartMesh>();
            foreach (PartMesh part in parts)
            {
                Vector3 randomPoint = new Vector3(
                    Random.Range(part.Bounds.min.x, part.Bounds.max.x),
                    Random.Range(part.Bounds.min.y, part.Bounds.max.y),
                    Random.Range(part.Bounds.min.z, part.Bounds.max.z)
                );
                Plane plane = new Plane(Random.onUnitSphere, randomPoint);

                subParts.Add(meshCutter.GenerateMesh(part, plane, true));
                subParts.Add(meshCutter.GenerateMesh(part, plane, false));
            }
            parts = subParts;
        }

        foreach (PartMesh part in parts)
        {
            part.MakeGameObject(this, meshRenderer.materials);
            forceApplier.ApplyForce(part.GameObject, part.Bounds.center);
        }

        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Destroyer"))
        {
            DestroyMesh();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Destroyer"))
        {
            DestroyMesh();
        }
    }
}