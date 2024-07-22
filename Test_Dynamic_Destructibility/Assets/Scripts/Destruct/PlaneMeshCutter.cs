using UnityEngine;

public class PlaneMeshCutter : IMeshCutter
{
    private bool edgeSet = false;
    private Vector3 edgeVertex = Vector3.zero;
    private Vector2 edgeUV = Vector2.zero;
    private Plane edgePlane = new Plane();

    public PartMesh GenerateMesh(PartMesh original, Plane plane, bool left)
    {
        PartMesh partMesh = new PartMesh();
        edgeSet = false;

        for (int i = 0; i < original.Triangles.Length; i++)
        {
            int[] triangles = original.Triangles[i];

            for (int j = 0; j < triangles.Length; j += 3)
            {
                bool sideA = plane.GetSide(original.Vertices[triangles[j]]) == left;
                bool sideB = plane.GetSide(original.Vertices[triangles[j + 1]]) == left;
                bool sideC = plane.GetSide(original.Vertices[triangles[j + 2]]) == left;

                int sideCount = (sideA ? 1 : 0) + (sideB ? 1 : 0) + (sideC ? 1 : 0);

                if (sideCount == 0) continue;

                if (sideCount == 3)
                {
                    partMesh.AddTriangle(i,
                        original.Vertices[triangles[j]], original.Vertices[triangles[j + 1]], original.Vertices[triangles[j + 2]],
                        original.Normals[triangles[j]], original.Normals[triangles[j + 1]], original.Normals[triangles[j + 2]],
                        original.UV[triangles[j]], original.UV[triangles[j + 1]], original.UV[triangles[j + 2]]);
                    continue;
                }

                int singleIndex = sideB == sideC ? 0 : sideA == sideC ? 1 : 2;

                Ray ray1 = new Ray(original.Vertices[triangles[j + singleIndex]],
                    original.Vertices[triangles[j + (singleIndex + 1) % 3]] - original.Vertices[triangles[j + singleIndex]]);
                Ray ray2 = new Ray(original.Vertices[triangles[j + singleIndex]],
                    original.Vertices[triangles[j + (singleIndex + 2) % 3]] - original.Vertices[triangles[j + singleIndex]]);

                if (plane.Raycast(ray1, out float enter1) && plane.Raycast(ray2, out float enter2))
                {
                    Vector2 uv1 = Vector2.Lerp(original.UV[triangles[j + singleIndex]], original.UV[triangles[j + (singleIndex + 1) % 3]], enter1);
                    Vector2 uv2 = Vector2.Lerp(original.UV[triangles[j + singleIndex]], original.UV[triangles[j + (singleIndex + 2) % 3]], enter2);

                    AddEdge(i, partMesh,
                        left ? -plane.normal : plane.normal,
                        ray1.origin + ray1.direction * enter1,
                        ray2.origin + ray2.direction * enter2,
                        uv1, uv2);

                    if (sideCount == 1)
                    {
                        partMesh.AddTriangle(i,
                            original.Vertices[triangles[j + singleIndex]],
                            ray1.origin + ray1.direction * enter1,
                            ray2.origin + ray2.direction * enter2,
                            original.Normals[triangles[j + singleIndex]],
                            Vector3.Lerp(original.Normals[triangles[j + singleIndex]], original.Normals[triangles[j + (singleIndex + 1) % 3]], enter1),
                            Vector3.Lerp(original.Normals[triangles[j + singleIndex]], original.Normals[triangles[j + (singleIndex + 2) % 3]], enter2),
                            original.UV[triangles[j + singleIndex]],
                            uv1, uv2);
                    }
                    else if (sideCount == 2)
                    {
                        partMesh.AddTriangle(i,
                            ray1.origin + ray1.direction * enter1,
                            original.Vertices[triangles[j + (singleIndex + 1) % 3]],
                            original.Vertices[triangles[j + (singleIndex + 2) % 3]],
                            Vector3.Lerp(original.Normals[triangles[j + singleIndex]], original.Normals[triangles[j + (singleIndex + 1) % 3]], enter1),
                            original.Normals[triangles[j + (singleIndex + 1) % 3]],
                            original.Normals[triangles[j + (singleIndex + 2) % 3]],
                            uv1,
                            original.UV[triangles[j + (singleIndex + 1) % 3]],
                            original.UV[triangles[j + (singleIndex + 2) % 3]]);
                        partMesh.AddTriangle(i,
                            ray1.origin + ray1.direction * enter1,
                            original.Vertices[triangles[j + (singleIndex + 2) % 3]],
                            ray2.origin + ray2.direction * enter2,
                            Vector3.Lerp(original.Normals[triangles[j + singleIndex]], original.Normals[triangles[j + (singleIndex + 1) % 3]], enter1),
                            original.Normals[triangles[j + (singleIndex + 2) % 3]],
                            Vector3.Lerp(original.Normals[triangles[j + singleIndex]], original.Normals[triangles[j + (singleIndex + 2) % 3]], enter2),
                            uv1,
                            original.UV[triangles[j + (singleIndex + 2) % 3]],
                            uv2);
                    }
                }
            }
        }

        partMesh.FillArrays();
        return partMesh;
    }

    private void AddEdge(int subMesh, PartMesh partMesh, Vector3 normal, Vector3 vertex1, Vector3 vertex2, Vector2 uv1, Vector2 uv2)
    {
        if (!edgeSet)
        {
            edgeSet = true;
            edgeVertex = vertex1;
            edgeUV = uv1;
        }
        else
        {
            edgePlane.Set3Points(edgeVertex, vertex1, vertex2);
            partMesh.AddTriangle(subMesh, edgeVertex, edgePlane.GetSide(edgeVertex + normal) ? vertex1 : vertex2, edgePlane.GetSide(edgeVertex + normal) ? vertex2 : vertex1, normal, normal, normal, edgeUV, uv1, uv2);
        }
    }
}