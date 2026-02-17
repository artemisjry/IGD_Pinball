using UnityEngine;

public class BallScoring : MonoBehaviour
{
    public int bumperPoints = 500;
    public int slingshotPoints = 1000;
    public int floatiePoints = 5000;
    public int portalPoints = 1000;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (ScoreManager.Instance == null) return;

        var tag = collision.collider.tag;

        if (tag == "Bumper")
        {
            ScoreManager.Instance.AddScore(bumperPoints);
            AudioManager.Instance.PlayCollision();
        }
        else if (tag == "Slingshot")
        {
            ScoreManager.Instance.AddScore(slingshotPoints);
            AudioManager.Instance.PlayCollision();
        }
        else if (tag == "Floatie")
        {
            ScoreManager.Instance.AddScore(floatiePoints);
            AudioManager.Instance.PlayCollision();
        }
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (ScoreManager.Instance == null) return;

        if (other.CompareTag("Portal"))
            ScoreManager.Instance.AddScore(portalPoints);
    }
}
