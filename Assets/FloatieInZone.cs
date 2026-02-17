using UnityEngine;

public class FloatieInZone : MonoBehaviour
{
    public Collider2D zone;
    public LayerMask ballLayer;

    public float targetJitterRadius = 1.1f;
    public float retargetIntervalMin = 0.6f;
    public float retargetIntervalMax = 1.1f;

    public float moveForce = 10f;
    public float maxSpeed = 5f;

    public float wanderForce = 0.4f;

    public float ballPushImpulse = 12f;
    public float maxBallSpeed = 95f;

    public Transform visual;
    public float pulseScale = 1.25f;
    public float pulseSpeed = 20f;

    public float stuckSpeedThreshold = 0.12f;
    public float stuckTime = 0.45f;
    public float unstuckImpulseMultiplier = 1.2f;

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

        Vector2 desiredPos = zone.OverlapPoint(pos) ? target : zoneCenter;
        Vector2 to = desiredPos - pos;

        Vector2 desiredVel = Vector2.zero;
        float d = to.magnitude;
        if (d > 0.001f)
            desiredVel = (to / d) * maxSpeed;

        Vector2 dv = desiredVel - rb.linearVelocity;
        Vector2 force = dv * (moveForce * rb.mass);

        float fm = force.magnitude;
        float maxF = moveForce * rb.mass;
        if (fm > maxF) force = force / fm * maxF;

        rb.AddForce(force, ForceMode2D.Force);

        if (wanderForce > 0f)
            rb.AddForce(Random.insideUnitCircle * wanderForce, ForceMode2D.Force);

        float spd = rb.linearVelocity.magnitude;

        if (spd < stuckSpeedThreshold)
            stuckTimer += Time.fixedDeltaTime;
        else
            stuckTimer = 0f;

        if (stuckTimer > stuckTime)
        {
            PickNewTarget(true);

            Vector2 dir = (target - rb.position);
            float dm = dir.magnitude;
            if (dm > 0.001f)
                dir /= dm;
            else
                dir = Random.insideUnitCircle.normalized;

            rb.AddForce(dir * (moveForce * unstuckImpulseMultiplier), ForceMode2D.Impulse);
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
