using UnityEngine;

public class ExplosiveForceApplier : IForceApplier
{
    private readonly float explodeForce;

    public ExplosiveForceApplier(float explodeForce)
    {
        this.explodeForce = explodeForce;
    }

    public void ApplyForce(GameObject gameObject, Vector3 force)
    {
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(force * explodeForce, ForceMode.Impulse);
        }
    }
}
