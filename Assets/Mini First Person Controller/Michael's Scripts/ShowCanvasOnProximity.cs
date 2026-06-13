using System.Collections;
using UnityEngine;

public class ShowCanvasOnProximity : MonoBehaviour
{
    [Header("References (Auto-assigned)")]
    [Tooltip("The Canvas child object.")]
    public GameObject worldCanvas;

    [Tooltip("The Cylinder child object (used for visual radius).")]
    public GameObject cylinderObstacle;

    private CanvasGroup canvasGroup;

    [Header("Settings")]
    [Tooltip("The tag of your player character.")]
    public string playerTag = "Player";

    [Tooltip("Time to fade in/out.")]
    public float fadeDuration = 0.5f;

    [Tooltip("How long after leaving before fading out.")]
    public float exitDelay = 5f;

    // Track the currently active trigger globally
    private static ShowCanvasOnProximity activeTrigger;

    private Coroutine hideCoroutine;
    private Coroutine fadeCoroutine;

    private SC_RigidbodyWalker playerController;

    void Reset()
    {
        Canvas canvasComponent = GetComponentInChildren<Canvas>(true);

        if (canvasComponent != null)
        {
            worldCanvas = canvasComponent.gameObject;
        }

        MeshFilter[] meshes = GetComponentsInChildren<MeshFilter>(true);

        foreach (var mesh in meshes)
        {
            if (mesh.name.ToLower().Contains("cylinder"))
            {
                cylinderObstacle = mesh.gameObject;
                break;
            }
        }
    }

    void Start()
    {
        playerController = FindFirstObjectByType<SC_RigidbodyWalker>();

        if (worldCanvas == null)
            Reset();

        if (worldCanvas != null)
        {
            worldCanvas.SetActive(true);

            canvasGroup = worldCanvas.GetComponent<CanvasGroup>();

            if (canvasGroup == null)
            {
                canvasGroup = worldCanvas.AddComponent<CanvasGroup>();
            }

            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    void LateUpdate()
    {
        if (worldCanvas == null || canvasGroup == null)
            return;

        if (canvasGroup.alpha > 0f && Camera.main != null)
        {
            Vector3 cameraEulerAngles = Camera.main.transform.eulerAngles;

            worldCanvas.transform.rotation =
                Quaternion.Euler(0f, cameraEulerAngles.y, 0f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag))
            return;

        Debug.Log($"Player entered trigger: {name}");

        // Cancel any pending hide
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }

        // If another trigger is active, hide it immediately
        if (activeTrigger != null && activeTrigger != this)
        {
            activeTrigger.HideImmediately();
        }

        activeTrigger = this;

        Show();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag))
            return;

        Debug.Log($"Player exited trigger: {name}");

        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }

        hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(exitDelay);

        yield return FadeCanvas(0f);

        if (activeTrigger == this)
        {
            activeTrigger = null;
        }
    }

    private void Show()
    {
        if (playerController != null)
        {
            playerController.DisableMouseLook();
        }

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeCanvas(1f));
    }

    private void HideImmediately()
    {
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    private IEnumerator FadeCanvas(float targetAlpha)
    {
        float startAlpha = canvasGroup.alpha;
        float elapsed = 0f;

        if (targetAlpha > 0f)
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / fadeDuration);

            // Smooth Ease In-Out
            t = t * t * (3f - 2f * t);

            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);

            yield return null;
        }

        canvasGroup.alpha = targetAlpha;

        if (targetAlpha <= 0f)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }
}