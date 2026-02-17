using UnityEngine;

public class DrainZone : MonoBehaviour
{
    public BallRespawnManager manager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Rigidbody2D rb = other.attachedRigidbody;
        if (rb == null) return;
        if (manager == null) return;

        if (rb == manager.ball)
            manager.OnBallDrained();
    }
}
