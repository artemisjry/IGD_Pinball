using UnityEngine;

public class BallRespawnManager : MonoBehaviour
{
    public Rigidbody2D ball;
    public Transform spawnPoint;

    public float respawnDelay = 0.5f;
    public float ballSaveTime = 0.75f;

    private float respawnAt = -1f;
    private float ignoreDrainUntil = -1f;

    private void Update()
    {
        if (respawnAt > 0f && Time.time >= respawnAt)
        {
            DoRespawn();
            respawnAt = -1f;
        }
    }

    public void OnBallDrained()
    {
        if (Time.time < ignoreDrainUntil) return;
        if (respawnAt > 0f) return;

        respawnAt = Time.time + respawnDelay;
    }

    private void DoRespawn()
    {
        if (ball == null || spawnPoint == null) return;

        ball.linearVelocity = Vector2.zero;
        ball.angularVelocity = 0f;

        ball.position = spawnPoint.position;
        ball.rotation = 0f;

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.ResetScore();

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayRespawn();


        ignoreDrainUntil = Time.time + ballSaveTime;
    }

}
