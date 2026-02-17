using UnityEngine;

public class PinballSlingshot : MonoBehaviour
{
    public float kickSpeed = 22f;
    public float minKickSpeed = 18f;
    public float maxBallSpeed = 70f;

    public float cooldown = 0.1f;

    public Transform kickDirection;

    public float pulseScale = 1.2f;
    public float pulseSpeed = 20f;

    private float nextAllowedTime;

    private Vector3 baseScale;
    private float pulseAmount;

    private void Awake()
    {
        baseScale = transform.localScale;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (Time.time < nextAllowedTime) return;

        Rigidbody2D rb = other.attachedRigidbody;
        if (rb == null) return;

        Vector2 dir;

        if (kickDirection != null)
            dir = kickDirection.up;
        else
            dir = ((Vector2)rb.position - (Vector2)transform.position).normalized;

        float incoming = rb.linearVelocity.magnitude;
        float targetSpeed = Mathf.Max(minKickSpeed, incoming + kickSpeed);

        Vector2 newV = dir.normalized * targetSpeed;

        if (newV.magnitude > maxBallSpeed)
            newV = newV.normalized * maxBallSpeed;

        rb.linearVelocity = newV;

        pulseAmount = 1f;
        nextAllowedTime = Time.time + cooldown;
    }

    private void Update()
    {
        if (pulseAmount <= 0f) return;

        pulseAmount = Mathf.MoveTowards(pulseAmount, 0f, pulseSpeed * Time.deltaTime);

        float t = pulseAmount;
        float eased = t * t * (3f - 2f * t);

        float scale = Mathf.Lerp(1f, pulseScale, eased);
        transform.localScale = baseScale * scale;
    }
}
