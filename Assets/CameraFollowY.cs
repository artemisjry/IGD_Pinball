using UnityEngine;

public class CameraFollowY : MonoBehaviour
{
    public Transform target;
    public float smoothTime = 0.2f;

    public float minY;
    public float maxY;

    float velocityY;

    void LateUpdate()
    {
        if (target == null) return;

        float targetY = Mathf.SmoothDamp(
            transform.position.y,
            target.position.y,
            ref velocityY,
            smoothTime
        );

        targetY = Mathf.Clamp(targetY, minY, maxY);

        transform.position = new Vector3(
            transform.position.x,
            targetY,
            transform.position.z
        );
    }
}
