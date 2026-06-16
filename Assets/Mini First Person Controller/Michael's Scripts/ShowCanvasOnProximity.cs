using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShowCanvasOnProximity : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The separately referenced Canvas object.")]
    public GameObject worldCanvas;

    [Header("External App Launcher")]
[Tooltip("Reference to the script that launches fullscreen batch files.")]
public OpenAppFullScreen appLauncher;

    [Tooltip("The Cylinder child object (used for visual radius).")]
    public GameObject cylinderObstacle;

    [Header("Game Data")]
    [Tooltip("The game this trigger zone should display when the player enters.")]
    public GameScriptableObject Game;

    [Header("Canvas UI References (on worldCanvas)")]
    [SerializeField] private TextMeshProUGUI gameText;
    [SerializeField] private Image gameImage;
    [SerializeField] private Button playButton;

    private CanvasGroup canvasGroup;

    [Header("Settings")]
    [Tooltip("The tag of your player character.")]
    public string playerTag = "Player";

    [Tooltip("Time to fade in/out.")]
    public float fadeDuration = 0.5f;

    [Tooltip("How long after leaving before fading out.")]
    public float exitDelay = 5f;

    [Header("GameObject Bobbing & Rotation")]
    [Tooltip("How fast this GameObject rotates to face the player.")]
    public float rotationSpeed = 5f;

    [Tooltip("How fast this GameObject floats up and down.")]
    public float bobSpeed = 2f;

    [Tooltip("How high/low this GameObject floats.")]
    public float bobAmount = 0.05f;

    [Header("Weeble Wobble Settings")]
    [Tooltip("Maximum tilt angle in degrees.")]
    public float maxWobbleAngle = 3.0f;

    [Tooltip("How fast it rocks back and forth.")]
    public float wobbleSpeed = 2.5f;

    // Track the currently active trigger globally
    private static ShowCanvasOnProximity activeTrigger;

    private Coroutine hideCoroutine;
    private Coroutine fadeCoroutine;

    private SC_RigidbodyWalker playerController;
    
    // Captured structural baselines (preserves your spherical placement!)
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    // Unique random seeds so every instance behaves differently
    private float randomTimeOffset;
    private float randomXDirection;
    private float randomZDirection;

    // Interpolated modifier for player tracking
    private float lookYOffset;

    void Reset()
    {
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

        // IMPORTANT: Capture exact position and rotation as placed on the 3D sphere
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        // Generate unique offsets so every single instance acts completely independently
        randomTimeOffset = Random.Range(0f, 100f);
        randomXDirection = Random.Range(0.7f, 1.3f);
        randomZDirection = Random.Range(0.7f, 1.3f);

        if (worldCanvas != null)
        {
            if (gameText == null) gameText = worldCanvas.GetComponentInChildren<TextMeshProUGUI>(true);
            if (gameImage == null) gameImage = worldCanvas.GetComponentInChildren<Image>(true);
            if (playButton == null) playButton = worldCanvas.GetComponentInChildren<Button>(true);

            worldCanvas.SetActive(true);
            canvasGroup = worldCanvas.GetComponent<CanvasGroup>() ?? worldCanvas.AddComponent<CanvasGroup>();

            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    void LateUpdate()
    {
        // 1. BOBBING EFFECT (Relative to its unique sphere alignment vector)
        // Using transform.up preserves the custom alignment pointing directly out from the sphere center
        float bobOffset = Mathf.Sin((Time.time + randomTimeOffset) * bobSpeed) * bobAmount;
        transform.position = initialPosition + (initialRotation * Vector3.up * bobOffset);

        // 2. FACE THE PLAYER (Calculated relative to the initial surface orientation)
        float targetYOffset = 0f;

        if (playerController != null && canvasGroup != null && canvasGroup.alpha > 0f)
        {
            // Project direction onto the local horizon plane of the object on the sphere
            Vector3 directionToPlayer = playerController.transform.position - transform.position;
            Vector3 localDirection = transform.InverseTransformDirection(directionToPlayer);
            
            // Flatten the direction on the local surface plane (X/Z in local space)
            localDirection.y = 0f; 

            if (localDirection != Vector3.zero)
            {
                // Determine the angle difference needed on the Y-axis to look at the player
                targetYOffset = Mathf.Atan2(localDirection.x, localDirection.z) * Mathf.Rad2Deg;
            }
        }

        // Smoothly interpolate the turning angle offset
        lookYOffset = Mathf.LerpAngle(lookYOffset, targetYOffset, Time.deltaTime * rotationSpeed);

        // 3. WEEBLE WOBBLE CALCULATIONS
        float currentWobbleTime = Time.time + randomTimeOffset;
        float wobbleX = Mathf.Sin(currentWobbleTime * wobbleSpeed * randomXDirection) * maxWobbleAngle;
        float wobbleZ = Mathf.Cos(currentWobbleTime * wobbleSpeed * randomZDirection) * maxWobbleAngle;

        // 4. COMBINE MOTIONS SEQUENTIALLY
        // Start with original placement -> Add smooth player tracking -> Add local procedural wobble
        Quaternion trackingRotation = initialRotation * Quaternion.Euler(0f, lookYOffset, 0f);
        Quaternion finalWobbleRotation = trackingRotation * Quaternion.Euler(wobbleX, 0f, wobbleZ);

        transform.rotation = finalWobbleRotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }

        if (activeTrigger != null && activeTrigger != this)
        {
            activeTrigger.HideImmediately();
        }

        activeTrigger = this;
        UpdateCanvasUI();
        Show();
    }

    private void UpdateCanvasUI()
    {
        if (Game == null) return;

        if (gameImage != null) gameImage.sprite = Game.GameImage;
        if (gameText != null) gameText.text = $"{Game.GameName}\nMade by {Game.Author}";
        
        if (playButton != null)
        {
            playButton.onClick.RemoveAllListeners();
            // 2. Change the listener here to call OpenBatchWithParamter instead
            playButton.onClick.AddListener(() => {
            if (appLauncher != null)
            {
                // Convert the integer index to a string (e.g., 0 becomes "0") 
                // and pass it to your batch executor
                string parameterToSend = Game.batchAppIndex.ToString();
                appLauncher.OpenBatchwithParamter(parameterToSend);
            }
            else
            {
                Debug.LogError("App Launcher reference missing on " + gameObject.name);
            }
        });
        }
    }

    public void OpenGameLink()
    {
        if (Game == null || string.IsNullOrWhiteSpace(Game.Link)) return;
        Application.OpenURL(Game.Link);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (hideCoroutine != null) StopCoroutine(hideCoroutine);

        if (playerController != null)
        {
            playerController.EnableMouseLook();
        }

        hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(exitDelay);
        yield return FadeCanvas(0f);

        if (activeTrigger == this) activeTrigger = null;
    }

    private void Show()
    {
        if (playerController != null) playerController.DisableMouseLook();
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        
        fadeCoroutine = StartCoroutine(FadeCanvas(1f));
    }

    private void HideImmediately()
    {
        if (hideCoroutine != null) StopCoroutine(hideCoroutine);
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);

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
            t = t * t * (3f - 2f * t); // Smoothstep ease

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