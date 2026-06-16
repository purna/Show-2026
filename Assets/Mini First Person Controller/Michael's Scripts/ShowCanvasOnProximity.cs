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
    [SerializeField] private TextMeshProUGUI gameTextAuthor;
    [SerializeField] private TextMeshProUGUI gameText0;
    [SerializeField] private TextMeshProUGUI gameText1;
    [SerializeField] private Image gameImage;
    [SerializeField] private Button playButton0;
    [SerializeField] private Button playButton1;

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

    private static ShowCanvasOnProximity activeTrigger;

    private Coroutine hideCoroutine;
    private Coroutine fadeCoroutine;

    private SC_RigidbodyWalker playerController;
    
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private float randomTimeOffset;
    private float randomXDirection;
    private float randomZDirection;

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

        initialPosition = transform.position;
        initialRotation = transform.rotation;

        randomTimeOffset = Random.Range(0f, 100f);
        randomXDirection = Random.Range(0.7f, 1.3f);
        randomZDirection = Random.Range(0.7f, 1.3f);

        if (worldCanvas != null)
        {
            if (gameText0 == null) gameText0 = worldCanvas.GetComponentInChildren<TextMeshProUGUI>(true);
            if (gameText1 == null) gameText1 = worldCanvas.GetComponentInChildren<TextMeshProUGUI>(true);
            if (gameTextAuthor == null) gameTextAuthor = worldCanvas.GetComponentInChildren<TextMeshProUGUI>(true);
            if (gameImage == null) gameImage = worldCanvas.GetComponentInChildren<Image>(true);
            if (playButton0 == null) playButton0 = worldCanvas.GetComponentInChildren<Button>(true);
            if (playButton1 == null) playButton1 = worldCanvas.GetComponentInChildren<Button>(true);

            worldCanvas.SetActive(true);
            canvasGroup = worldCanvas.GetComponent<CanvasGroup>() ?? worldCanvas.AddComponent<CanvasGroup>();

            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    void LateUpdate()
    {
        float bobOffset = Mathf.Sin((Time.time + randomTimeOffset) * bobSpeed) * bobAmount;
        transform.position = initialPosition + (initialRotation * Vector3.up * bobOffset);

        float targetYOffset = 0f;

        if (playerController != null && canvasGroup != null && canvasGroup.alpha > 0f)
        {
            Vector3 directionToPlayer = playerController.transform.position - transform.position;
            Vector3 localDirection = transform.InverseTransformDirection(directionToPlayer);
            localDirection.y = 0f; 

            if (localDirection != Vector3.zero)
            {
                targetYOffset = Mathf.Atan2(localDirection.x, localDirection.z) * Mathf.Rad2Deg;
            }
        }

        lookYOffset = Mathf.LerpAngle(lookYOffset, targetYOffset, Time.deltaTime * rotationSpeed);

        float currentWobbleTime = Time.time + randomTimeOffset;
        float wobbleX = Mathf.Sin(currentWobbleTime * wobbleSpeed * randomXDirection) * maxWobbleAngle;
        float wobbleZ = Mathf.Cos(currentWobbleTime * wobbleSpeed * randomZDirection) * maxWobbleAngle;

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
        if (gameTextAuthor != null) gameTextAuthor.text = $"Made by {Game.Author}";

        // --- BUTTON 0 ---
        if (gameText0 != null) gameText0.text = Game.GameName0;
        if (playButton0 != null)
        {
            playButton0.transform.parent.gameObject.SetActive(true);
            playButton0.onClick.RemoveAllListeners();
            playButton0.onClick.AddListener(() => {
                // New debug message
                Debug.Log($"<color=cyan>[UI Click]</color> Button 0 Clicked for game: {Game.GameName0}. Index payload: {Game.batchAppIndex0}");
                ExecuteAppLaunch(Game.batchAppIndex0);
            });
        }

        // --- BUTTON 1 ---
        if (playButton1 != null)
        {
            if (string.IsNullOrWhiteSpace(Game.GameName1))
            {
                playButton1.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                playButton1.transform.parent.gameObject.SetActive(true);

                if (gameText1 != null) gameText1.text = Game.GameName1;

                playButton1.onClick.RemoveAllListeners();
                playButton1.onClick.AddListener(() => {
                    // New debug message
                    Debug.Log($"<color=orange>[UI Click]</color> Button 1 Clicked for game: {Game.GameName1}. Index payload: {Game.batchAppIndex1}");
                    ExecuteAppLaunch(Game.batchAppIndex1);
                });
            }
        }
    }

    private void ExecuteAppLaunch(int index)
    {
        if (appLauncher != null)
        {
            appLauncher.OpenBatchwithParamter(index.ToString());
        }
        else
        {
            Debug.LogError($"App Launcher system dependency missing on: {gameObject.name}");
        }
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