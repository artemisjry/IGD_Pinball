using System.Collections.Generic;
using UnityEngine;

public class StarTarget : MonoBehaviour
{
    static readonly List<StarTarget> all = new List<StarTarget>();
    static int sequenceIndex;

    public Color inactiveColor = Color.white;
    public Color activeColor = Color.green;

    SpriteRenderer sr;
    bool activated;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = inactiveColor;
        all.Add(this);
    }

    void OnDestroy()
    {
        all.Remove(this);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (activated) return;
        if (!other.CompareTag("Ball")) return;

        if (other.transform.position.y <= transform.position.y) return;

        activated = true;

        if (sr != null) sr.color = activeColor;

        int points = 200 << sequenceIndex;

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.AddScore(points);

        sequenceIndex++;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayScore();
    }

    void ResetState()
    {
        activated = false;
        if (sr != null) sr.color = inactiveColor;
    }

    public static void ResetAllTargets()
    {
        sequenceIndex = 0;

        for (int i = 0; i < all.Count; i++)
            all[i].ResetState();
    }
}
