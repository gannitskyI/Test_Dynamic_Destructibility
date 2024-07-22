using UnityEngine;

public interface IMeshCutter
{
    PartMesh GenerateMesh(PartMesh original, Plane plane, bool left);
}