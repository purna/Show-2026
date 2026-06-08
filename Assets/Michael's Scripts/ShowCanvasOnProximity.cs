using UnityEngine;

public class ShowCanvasOnProximity : MonoBehaviour
{
    [Header("References (Auto-assigned)")]
    [Tooltip("The Canvas child object.")]
    public GameObject worldCanvas;

    [Tooltip("The Cylinder child object (used for visual radius).")]
    public GameObject cylinderObstacle;

    // ✔️ ADDED: Reference to handle opacity controls
    private CanvasGroup canvasGroup;

    [Header("Settings")]
    [Tooltip("The tag of your player character.")]
    public string playerTag = "Player";

    /// <summary>
    /// This runs automatically when you attach the script or click "Reset" in the Inspector.
    /// It automatically finds the child Canvas and Cylinder objects.
    /// </summary>
    void Reset()
    {
        // Find the Canvas component in children and get its GameObject
        Canvas canvasComponent = GetComponentInChildren<Canvas>(true);
        if (canvasComponent != null)
        {
            worldCanvas = canvasComponent.gameObject;
        }

        // Find the Cylinder by searching for a child with a CapsuleCollider or MeshFilter named cylinder
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
        // Double-check components if Reset wasn't triggered
        if (worldCanvas == null) Reset();

        if (worldCanvas != null)
        {
            // ✔️ FIXED: Ensure the GameObject itself stays Active so it can update its rotation/scripts
            worldCanvas.SetActive(true);

            // ✔️ ADDED: Get the CanvasGroup or add one if it's missing from the canvas
            canvasGroup = worldCanvas.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = worldCanvas.AddComponent<CanvasGroup>();
            }

            // ✔️ FIXED: Initialize as completely invisible and non-interactive
            SetCanvasGroupState(false);
        }
    }

    void LateUpdate()
    {
        // ✔️ FIXED: Check alpha instead of activeSelf so it rotates while faded in
        if (worldCanvas != null && canvasGroup != null && canvasGroup.alpha > 0f)
        {
            if (Camera.main != null)
            {
                // Rotates the canvas horizontally so it perfectly faces the main camera
                Vector3 cameraEulerAngles = Camera.main.transform.eulerAngles;
                Quaternion targetHorizontalRotation = Quaternion.Euler(0f, cameraEulerAngles.y, 0f);
                worldCanvas.transform.rotation = targetHorizontalRotation;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // ✔️ ADDED: General trigger debug log to see if ANYTHING is hitting it
        Debug.Log($"[Trigger Enter] Something entered the zone: '{other.name}' with tag '{other.tag}'", other.gameObject);

        // Check if the specific object entering the trigger radius is the player
        if (other.CompareTag(playerTag))
        {
            // ✔️ ADDED: Specific validation debug log
            Debug.Log($"SUCCESS: Player '{other.name}' has entered the zone! Fading Canvas in.", gameObject);
            
            SetCanvasGroupState(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the object leaving the trigger radius is the player
        if (other.CompareTag(playerTag))
        {
            // ✔️ ADDED: Specific validation debug log
            Debug.Log($"SUCCESS: Player '{other.name}' has exited the zone! Fading Canvas out.", gameObject);
            
            SetCanvasGroupState(false);
        }
    }

    /// <summary>
    /// Helper method to change opacity and UI click interactivity instantly.
    /// </summary>
    private void SetCanvasGroupState(bool isVisible)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = isVisible ? 1f : 0f;
            canvasGroup.interactable = isVisible;
            canvasGroup.blocksRaycasts = isVisible;
        }
    }
}