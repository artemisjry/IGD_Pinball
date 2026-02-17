using UnityEngine;
using UnityEngine.InputSystem;

public class FlipperController : MonoBehaviour
{
    public InputActionReference flipAction;

    public float upMotorSpeed = 1200f;
    public float downMotorSpeed = -900f;
    public float maxMotorTorque = 12000f;

    public bool upIsMaxLimit = true;
    public float upCompletionAngleBuffer = 3f;

    private HingeJoint2D hinge;
    private bool held;
    private bool forcingUp;

    private void Awake()
    {
        hinge = GetComponent<HingeJoint2D>();
        hinge.useMotor = true;
    }

    private void OnEnable()
    {
        if (flipAction == null) return;
        flipAction.action.Enable();
        flipAction.action.started += OnStarted;
        flipAction.action.canceled += OnCanceled;
    }

    private void OnDisable()
    {
        if (flipAction == null) return;
        flipAction.action.started -= OnStarted;
        flipAction.action.canceled -= OnCanceled;
        flipAction.action.Disable();
    }

    private void OnStarted(InputAction.CallbackContext ctx)
    {
        held = true;
        forcingUp = true;
    }

    private void OnCanceled(InputAction.CallbackContext ctx)
    {
        held = false;
    }

    private void FixedUpdate()
    {
        JointAngleLimits2D lim = hinge.limits;
        float upAngle = upIsMaxLimit ? lim.max : lim.min;

        if (forcingUp && Mathf.Abs(hinge.jointAngle - upAngle) <= upCompletionAngleBuffer)
            forcingUp = false;

        bool shouldBeUp = forcingUp || held;

        JointMotor2D motor = hinge.motor;
        motor.maxMotorTorque = maxMotorTorque;
        motor.motorSpeed = shouldBeUp ? upMotorSpeed : downMotorSpeed;
        hinge.motor = motor;
    }
}
