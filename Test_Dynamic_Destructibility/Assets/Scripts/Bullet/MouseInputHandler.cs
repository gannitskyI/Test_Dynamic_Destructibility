using UnityEngine;

public class MouseInputHandler : IInputHandler
{
    public bool IsMouseButtonDown(int button)
    {
        return Input.GetMouseButtonDown(button);
    }

    public bool IsMouseButtonUp(int button)
    {
        return Input.GetMouseButtonUp(button);
    }

    public Vector3 GetMouseWorldPosition(Vector3 referencePosition)
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.WorldToScreenPoint(referencePosition).z;
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }
}