using UnityEngine;

public class LaunchGateAutoClose : MonoBehaviour
{
    public float closeMotorSpeed = -200f;

    public float openTorque = 40f;
    public float nearClosedTorque = 140f;

    public float nearClosedAngle = 6f;

    private HingeJoint2D hinge;

    private void Awake()
    {
        hinge = GetComponent<HingeJoint2D>();
        hinge.useMotor = true;
    }

    private void FixedUpdate()
    {
        float a = Mathf.Abs(hinge.jointAngle);

        JointMotor2D m = hinge.motor;
        m.motorSpeed = closeMotorSpeed;
        m.maxMotorTorque = (a <= nearClosedAngle) ? nearClosedTorque : openTorque;
        hinge.motor = m;
    }
}
