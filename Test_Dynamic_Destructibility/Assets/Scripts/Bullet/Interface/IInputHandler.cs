using UnityEngine;

public interface IInputHandler
{
    bool IsMouseButtonDown(int button);
    bool IsMouseButtonUp(int button);
    Vector3 GetMouseWorldPosition(Vector3 referencePosition);
}