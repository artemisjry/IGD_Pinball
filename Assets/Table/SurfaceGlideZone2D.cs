using System.Collections.Generic;
using UnityEngine;

public class SurfaceGlideZone2D : MonoBehaviour
{
    public LayerMask affectedLayers;

    public float airResistance = 0.35f;
    public float maxResistForce = 18f;

    public float angularDampingTorque = 18f;
    public float maxAngularTorque = 80f;

    public Vector2 current = Vector2.zero;
    public float currentResponsiveness = 3.5f;
    public float maxCurrentForce = 14f;

    private readonly HashSet<Rigidbody2D> inside = new HashSet<Rigidbody2D>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & affectedLayers) == 0) return;
        Rigidbody2D rb = other.attachedRigidbody;
        if (rb != null) inside.Add(rb);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Rigidbody2D rb = other.attachedRigidbody;
        if (rb != null) inside.Remove(rb);
    }

    private void FixedUpdate()
    {
        foreach (var rb in inside)
        {
            if (rb == null) continue;

            Vector2 v = rb.linearVelocity;
            float speed = v.magnitude;

            if (speed > 0.001f)
            {
                Vector2 dir = v / speed;
                float f = Mathf.Min(airResistance * speed, maxResistForce);
                rb.AddForce(-dir * f, ForceMode2D.Force);
            }

            float w = rb.angularVelocity;
            float torque = Mathf.Clamp(-w * angularDampingTorque, -maxAngularTorque, maxAngularTorque);
            rb.AddTorque(torque, ForceMode2D.Force);

            if (current != Vector2.zero)
            {
                Vector2 dv = current - v;
                Vector2 cf = dv * currentResponsiveness;
                float m = cf.magnitude;
                if (m > maxCurrentForce) cf = cf / m * maxCurrentForce;
                rb.AddForce(cf, ForceMode2D.Force);
            }
        }
    }
}
