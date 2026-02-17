using UnityEngine;

public class PinballBumper : MonoBehaviour
{
    public float minKickSpeed = 25f;
    public float addedKickSpeed = 20f;
    public float maxBallSpeed = 70f;

    public float pulseScale = 1.25f;
    public float pulseSpeed = 18f;

    private Vector3 baseScale;
    private float pulseAmount;

    private void Awake()
    {
        baseScale = transform.localScale;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        Rigidbody2D rb = col.rigidbody;
        if (rb == null) return;

        Vector2 normal = col.GetContact(0).normal;
        Vector2 v = rb.linearVelocity;

        float into = Vector2.Dot(v, -normal);
        float kick = Mathf.Max(minKickSpeed, into + addedKickSpeed);

        Vector2 newV = normal * kick;

        if (newV.magnitude > maxBallSpeed)
            newV = newV.normalized * maxBallSpeed;

        rb.linearVelocity = newV;

        pulseAmount = 1f;
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
