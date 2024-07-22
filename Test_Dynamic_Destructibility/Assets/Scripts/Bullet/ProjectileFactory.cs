using UnityEngine;

public class ProjectileFactory : IProjectileFactory
{
    private readonly GameObject projectilePrefab;
    private readonly Transform shootPoint;

    public ProjectileFactory(GameObject projectilePrefab, Transform shootPoint)
    {
        this.projectilePrefab = projectilePrefab;
        this.shootPoint = shootPoint;
    }

    public Projectile CreateProjectile()
    {
        GameObject projectileObject = Object.Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
        return projectileObject.AddComponent<Projectile>();
    }
}