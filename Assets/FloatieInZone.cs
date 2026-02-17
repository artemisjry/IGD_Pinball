using UnityEngine;

public class FloatieInZone : MonoBehaviour
{
    public Collider2D zone;
    public LayerMask ballLayer;

    public float driftSpeed = 1.4f;
    public float driftTurnRate = 0.35f;

    public float steeringForce = 0.8f;
    public float maxSteeringForce = 2.5f;

    public float boundaryForce = 4.0f;
    public float maxBoundaryForce = 10f;

    public float maxSpeed = 12f;

    public float ballPushImpulse = 10f;
    public float maxBallSpeed = 120f;

    public Transform visual;
    public float pulseScale = 1.25f;
    public float pulseSpeed = 20f;

    private Rigidbody2D rb;
    private Vector2 driftDir;
    private Vector2 zoneCenter;

    private Vector3 baseScale;
    private float pulseAmount;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (visual == null) visual = transform;
        baseScale = visual.localScale;

        driftDir = Random.insideUnitCircle.normalized;
        if (driftDir.sqrMagnitude < 0.001f) driftDir = Vector2.right;
    }

    private void Start()
    {
        if (zone != null)
            zoneCenter = zone.bounds.center;
    }

    private void FixedUpdate()
    {
        if (zone == null) return;

        zoneCenter = zone.bounds.center;

        driftDir = Vector2.Lerp(driftDir, Random.insideUnitCircle.normalized, driftTurnRate * Time.fixedDeltaTime);
        if (driftDir.sqrMagnitude < 0.001f) driftDir = Vector2.right;
        driftDir.Normalize();

        Vector2 v = rb.linearVelocity;
        float speed = v.magnitude;

        Vector2 desiredVel = driftDir * driftSpeed;
        Vector2 dv = desiredVel - v;

        Vector2 steer = dv * steeringForce;
        float sm = steer.magnitude;
        if (sm > maxSteeringForce) steer = steer / sm * maxSteeringForce;

        Vector2 pos = rb.position;
        bool inside = zone.OverlapPoint(pos);

        if (!inside)
        {
            Vector2 toCenter = zoneCenter - pos;
            float d = toCenter.magnitude;
            if (d > 0.001f)
            {
                Vector2 b = (toCenter / d) * boundaryForce;
                float bm = b.magnitude;
                if (bm > maxBoundaryForce) b = b / bm * maxBoundaryForce;
                rb.AddForce(b, ForceMode2D.Force);
            }
        }

        rb.AddForce(steer, ForceMode2D.Force);

        if (speed > maxSpeed)
            rb.linearVelocity = v.normalized * maxSpeed;

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
