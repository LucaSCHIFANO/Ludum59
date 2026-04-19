using UnityEngine;

public class CameraLast : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector2 camLimits;
    private void Update()
    {
        var newTarget = new Vector3(Mathf.Clamp(target.position.x, camLimits.x, camLimits.y), transform.position.y, transform.position.z);
        transform.position = newTarget;
    }
}
