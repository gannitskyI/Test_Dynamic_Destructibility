using UnityEngine;

public interface IForceApplier
{
    void ApplyForce(GameObject gameObject, Vector3 force);
}