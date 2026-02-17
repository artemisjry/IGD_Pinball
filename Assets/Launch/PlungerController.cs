using UnityEngine;
using UnityEngine.InputSystem;

public class PlungerController : MonoBehaviour
{
    public InputActionReference plungerAction;

    public ShooterBallZone shooterZone;

    public Transform plungerVisual;
    public Vector2 localPullAxis = Vector2.down;
    public bool invertVisualPull = false;

    public float maxPullDistance = 1.0f;
    public float pullSpeed = 1.8f;
    public float returnSpeed = 22f;

    public float minLaunchImpulse = 12f;
    public float maxLaunchImpulse = 40f;
    public float launchExponent = 2.2f;

    public Vector2 launchDirection = Vector2.up;

    private float pull01;
    private bool held;
    private bool wasHeld;

    private Vector3 visualStartLocalPos;

    private void Awake()
    {
        if (plungerVisual == null)
            plungerVisual = transform;

        visualStartLocalPos = plungerVisual.localPosition;
    }

    private void OnEnable()
    {
        if (plungerAction == null) return;

        plungerAction.action.Enable();
        plungerAction.action.started += OnStarted;
        plungerAction.action.canceled += OnCanceled;
    }

    private void OnDisable()
    {
        if (plungerAction == null) return;

        plungerAction.action.started -= OnStarted;
        plungerAction.action.canceled -= OnCanceled;
        plungerAction.action.Disable();
    }

    private void OnStarted(InputAction.CallbackContext ctx)
    {
        held = true;
    }

    private void OnCanceled(InputAction.CallbackContext ctx)
    {
        held = false;
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        if (held)
            pull01 = Mathf.Clamp01(pull01 + pullSpeed * dt);
        else
            pull01 = Mathf.MoveTowards(pull01, 0f, returnSpeed * dt);

        float eased = 1f - Mathf.Pow(1f - pull01, 2.5f);

        Vector2 axis = localPullAxis.normalized;
        float sign = invertVisualPull ? 1f : -1f;
        Vector3 offset = (Vector3)(axis * (sign * maxPullDistance * eased));
        plungerVisual.localPosition = visualStartLocalPos + offset;

        if (wasHeld && !held)
            TryLaunch();

        wasHeld = held;
    }

    private void TryLaunch()
    {
        if (shooterZone == null) return;
        if (!shooterZone.ballReady) return;
        if (shooterZone.ball == null) return;

        float shaped = Mathf.Pow(pull01, launchExponent);
        float impulse = Mathf.Lerp(minLaunchImpulse, maxLaunchImpulse, shaped);

        Rigidbody2D rb = shooterZone.ball;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(0f, rb.linearVelocity.y));
        rb.AddForce(launchDirection.normalized * impulse, ForceMode2D.Impulse);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayLaunch();


        pull01 = 0f;
    }
}
