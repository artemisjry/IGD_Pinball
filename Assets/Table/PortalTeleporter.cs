using System.Collections;
using UnityEngine;

public class PortalTeleporter : MonoBehaviour
{
    public PortalTeleporter targetPortal;
    public float teleportDelay = 0.15f;

    private bool isActive = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive) return;

        if (other.CompareTag("Ball"))
        {
            StartCoroutine(TeleportAfterDelay(other));
        }
    }

    private IEnumerator TeleportAfterDelay(Collider2D ball)
    {
        isActive = false;

        yield return new WaitForSeconds(teleportDelay);

        ball.transform.position = targetPortal.transform.position;

        targetPortal.DisableTemporarily();

        yield return new WaitForSeconds(0.1f);
        isActive = true;
    }

    private void DisableTemporarily()
    {
        StartCoroutine(DisableRoutine());
    }

    private IEnumerator DisableRoutine()
    {
        isActive = false;
        yield return new WaitForSeconds(0.2f);
        isActive = true;
    }
}
