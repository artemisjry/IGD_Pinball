using UnityEngine;

public class BallLimiter : MonoBehaviour
{
    public float maxSpeed = 25f;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        float s = rb.linearVelocity.magnitude;
        if (s > maxSpeed)
            rb.linearVelocity = rb.linearVelocity * (maxSpeed / s);
    }
}
