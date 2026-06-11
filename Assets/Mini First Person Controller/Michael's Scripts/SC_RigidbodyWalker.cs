using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]

public class SC_RigidbodyWalker : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 15.0f;
    public bool canJump = true;
    public float jumpHeight = 2.0f;
    private float maxVelocityChange = 10.0f;

    [Header("Camera & Rotation Settings")]
    public Camera playerCamera;
    public float lookSpeed = 2.0f;             // Sensitivity for Mouse
    public float keyboardTurnSpeed = 120.0f;   // Sensitivity for keyboard Arrow/AD turning
    public float lookXLimit = 60.0f;

    private bool grounded = false;
    private Rigidbody r;
    private Vector2 rotation = Vector2.zero;


    private bool allowLook = true;

    void Awake()
    {
        r = GetComponent<Rigidbody>();
        r.freezeRotation = true;
        r.useGravity = false;
        r.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rotation.y = transform.eulerAngles.y;

        // Lock cursor for desktop play
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // -------------------------------------------------------------------
        // 1. GATHER LOOK / TURN INPUT (Mouse + Keyboard Only - NO JOYSTICKS)
        // -------------------------------------------------------------------

        // Mouse Input
        float mouseLookX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseLookY = -Input.GetAxis("Mouse Y") * lookSpeed;

        // Keyboard Turning (Left/Right Arrow keys or A/D map to "Horizontal" by default)
        float keyboardTurnX = Input.GetAxis("Horizontal") * keyboardTurnSpeed * Time.deltaTime;

        // Combine inputs for looking left/right
        float totalLookX = mouseLookX + keyboardTurnX;
        float totalLookY = mouseLookY;

        // -------------------------------------------------------------------
        // 2. APPLY ROTATION
        // -------------------------------------------------------------------

        // Vertical Look (Pitch)
        rotation.x += totalLookY;
        rotation.x = Mathf.Clamp(rotation.x, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotation.x, 0, 0);

        // Horizontal Look (Yaw) - Turning on the spot
        Quaternion localRotation = Quaternion.Euler(0f, totalLookX, 0f);
        transform.rotation = transform.rotation * localRotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Game"))
        {
            GameImageScript gameScript = other.GetComponentInChildren<GameImageScript>();
            if (gameScript != null)
            {
                gameScript.LoadGame();
            }
        }
    }

    void FixedUpdate()
    {
        // Calculate forward direction relative to the camera orientation
        Vector3 forwardDir = Vector3.Cross(transform.up, -playerCamera.transform.right).normalized;

        // Forward/backward movement via Vertical axis (W/S or Up/Down arrows)
        Vector3 targetVelocity = (forwardDir * Input.GetAxis("Vertical")) * speed;

        // ✔️ PLANET SAFE: Maintain velocity relative to the player's local upright orientation
        Vector3 velocity = r.velocity;
        Vector3 velocityChange = targetVelocity - Vector3.ProjectOnPlane(velocity, transform.up);

        // Clamp the velocity change so the movement stays predictable
        velocityChange = Vector3.ClampMagnitude(velocityChange, maxVelocityChange);

        // Apply the force
        r.AddForce(velocityChange, ForceMode.VelocityChange);

        if (grounded)
        {
            if (Input.GetButton("Jump") && canJump)
            {
                // Jump upward relative to the planet's surface
                r.AddForce(transform.up * jumpHeight, ForceMode.VelocityChange);
            }
        }

        grounded = false;
    }

    void OnCollisionStay()
    {
        grounded = true;
    }

    public void DisableMouseLook()
    {
        allowLook = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void EnableMouseLook()
    {
        allowLook = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}