using System.Collections;
using UnityEngine;

public class ShowLocation : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Timing")]
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float visibleTime = 4f;
    [SerializeField] private float retriggerDelay = 4f;

    private bool canTrigger = true;

    private void Awake()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.gameObject.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canTrigger)
            return;

        // Optional: only trigger for player
        // if (!other.CompareTag("Player")) return;

        StartCoroutine(ShowRoutine());
    }

    private IEnumerator ShowRoutine()
    {
        canTrigger = false;

        yield return FadeCanvas(0f, 1f, fadeDuration);

        yield return new WaitForSeconds(visibleTime);

        yield return FadeCanvas(1f, 0f, fadeDuration);

        yield return new WaitForSeconds(retriggerDelay);

        canTrigger = true;
    }

    private IEnumerator FadeCanvas(float startAlpha, float endAlpha, float duration)
    {
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            float t = Mathf.Clamp01(time / duration);

            // Ease In-Out
            t = t * t * (3f - 2f * t);

            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);

            yield return null;
        }

        canvasGroup.alpha = endAlpha;
    }
}
