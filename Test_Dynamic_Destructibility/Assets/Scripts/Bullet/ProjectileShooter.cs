using UnityEngine;

public class ProjectileShooter : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab; // ������ �������
    [SerializeField] private Transform shootPoint; // ����� ��������� �������
    [SerializeField] private float maxStretch = 5f; // ������������ ���������
    [SerializeField] private float launchPower = 10f; // �������� �������
    [SerializeField] private float shootCooldown = 2f; // ����� ����� ���������

    private IProjectileFactory projectileFactory;
    private IInputHandler inputHandler;
    private Projectile currentProjectile;

    private Vector3 initialMousePosition;
    private bool isDragging = false;
    private float lastShootTime = -Mathf.Infinity;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        projectileFactory = new ProjectileFactory(projectilePrefab, shootPoint);
        inputHandler = new MouseInputHandler();
    }

    private void Update()
    {
        if (Time.time >= lastShootTime + shootCooldown)
        {
            if (inputHandler.IsMouseButtonDown(0))
            {
                StartDrag();
            }

            if (isDragging)
            {
                Drag();
            }

            if (inputHandler.IsMouseButtonUp(0) && isDragging)
            {
                Release();
            }
        }
    }

    private void StartDrag()
    {
        initialMousePosition = inputHandler.GetMouseWorldPosition(shootPoint.position);
        currentProjectile = projectileFactory.CreateProjectile();
        currentProjectile.SetKinematic(true);
        isDragging = true;
    }

    private void Drag()
    {
        Vector3 currentMousePosition = inputHandler.GetMouseWorldPosition(shootPoint.position);
        Vector3 dragVector = initialMousePosition - currentMousePosition;
        dragVector = Vector3.ClampMagnitude(dragVector, maxStretch);

        currentProjectile.SetPosition(shootPoint.position - dragVector);
    }

    private void Release()
    {
        isDragging = false;

        Vector3 releaseVector = shootPoint.position - currentProjectile.GetPosition();
        float stretchMagnitude = releaseVector.magnitude;

        currentProjectile.SetKinematic(false);
        currentProjectile.Launch(releaseVector.normalized * stretchMagnitude * launchPower);
        currentProjectile = null;

        lastShootTime = Time.time; // ��������� ����� ���������� ��������
    }
}
