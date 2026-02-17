using UnityEngine;

public class FloatieInZone : MonoBehaviour
{
    public Collider2D zone;
    public LayerMask ballLayer;

    public float targetJitterRadius = 0.8f;
    public float retargetIntervalMin = 0.6f;
    public float retargetIntervalMax = 1.4f;

    public float moveForce = 10f;
    public float maxSpeed = 6f;

    public float ballPushImpulse = 12f;
    public float maxBallSpeed = 95f;

    public Transform visual;
    public float pulseScale = 1.25f;
    public float pulseSpeed = 20f;

    private Rigidbody2D rb;
    private Vector2 zoneCenter;
    private Vector2 target;
    private float nextRetargetTime;

    private Vector3 baseScale;
    private float pulseAmount;

    private float stuckTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (visual == null) visual = transform;
        baseScale = visual.localScale;
    }

    private void Start()
    {
        if (zone != null)
        {
            zoneCenter = zone.bounds.center;
            PickNewTarget(true);
        }
    }

    private void FixedUpdate()
    {
        if (zone == null) return;

        if (Time.time >= nextRetargetTime)
            PickNewTarget(false);

        Vector2 pos = rb.position;

        Vector2 desired;
        if (!zone.OverlapPoint(pos))
            desired = zoneCenter - pos;
        else
            desired = target - pos;

        float d = desired.magnitude;
        if (d > 0.001f)
            rb.AddForce((desired / d) * moveForce, ForceMode2D.Force);

        float spd = rb.linearVelocity.magnitude;
        if (spd > maxSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;

        if (spd < 0.15f)
            stuckTimer += Time.fixedDeltaTime;
        else
            stuckTimer = 0f;

        if (stuckTimer > 0.35f)
        {
            PickNewTarget(true);
            rb.AddForce(Random.insideUnitCircle * (moveForce * 0.6f), ForceMode2D.Impulse);
            stuckTimer = 0f;
        }

        if (pulseAmount > 0f)
        {
            pulseAmount = Mathf.MoveTowards(pulseAmount, 0f, pulseSpeed * Time.fixedDeltaTime);
            float t = pulseAmount;
            float eased = t * t * (3f - 2f * t);
            float s = Mathf.Lerp(1f, pulseScale, eased);
            visual.localScale = baseScale * s;
        }
        else
        {
            visual.localScale = baseScale;
        }
    }

    private void PickNewTarget(bool immediate)
    {
        Vector2 c = zone.bounds.center;
        Vector2 r = Random.insideUnitCircle * targetJitterRadius;
        target = c + r;
        zoneCenter = c;

        float dt = Random.Range(retargetIntervalMin, retargetIntervalMax);
        nextRetargetTime = immediate ? Time.time : Time.time + dt;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        Rigidbody2D otherRb = col.rigidbody;
        if (otherRb == null) return;

        if (((1 << col.gameObject.layer) & ballLayer) == 0) return;

        Vector2 normal = col.GetContact(0).normal;

        Vector2 pushDir = -normal;
        otherRb.AddForce(pushDir * ballPushImpulse, ForceMode2D.Impulse);

        float newSpeed = otherRb.linearVelocity.magnitude;
        if (newSpeed > maxBallSpeed)
            otherRb.linearVelocity = otherRb.linearVelocity.normalized * maxBallSpeed;

        pulseAmount = 1f;
    }
}
