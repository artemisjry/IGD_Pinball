using UnityEngine;

public class ShooterBallZone : MonoBehaviour
{
    public Rigidbody2D ball;
    public bool ballReady;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (ball == null) return;
        Rigidbody2D rb = other.attachedRigidbody;
        if (rb == ball) ballReady = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (ball == null) return;
        Rigidbody2D rb = other.attachedRigidbody;
        if (rb == ball) ballReady = false;
    }
}
