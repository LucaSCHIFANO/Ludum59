using UnityEngine;

public class CameraLast : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector2 camLimits;

    /// <summary>
    /// Follow the main character. Can only move horizontally and can't go out of bounds
    /// </summary>
    private void Update()
    {
        var newTarget = new Vector3(Mathf.Clamp(target.position.x, camLimits.x, camLimits.y), transform.position.y, transform.position.z);
        transform.position = newTarget;
    }
}
