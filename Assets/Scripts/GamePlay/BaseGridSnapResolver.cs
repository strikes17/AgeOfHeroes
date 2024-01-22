using UnityEngine;

[ExecuteAlways]
public class BaseGridSnapResolver : MonoBehaviour
{
    public bool isEnabled = true;
    protected virtual void Update()
    {
        if(!isEnabled)
            return;
        Vector3 unclampedPosition = transform.position;
        unclampedPosition.x = Mathf.Floor(unclampedPosition.x);
        unclampedPosition.y = Mathf.Floor(unclampedPosition.y);
        unclampedPosition.z = Mathf.Floor(unclampedPosition.z);
        transform.position = unclampedPosition;
    }
}