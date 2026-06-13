using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SC_RigidbodyWalker : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 15f;
    public bool canJump = true;
    public float jumpHeight = 2f;
    private float maxVelocityChange = 10f;

    [Header("Camera")]
    public Transform cameraPivot;
    public float lookSpeed = 2f;
    public float keyboardTurnSpeed = 120f;
    public float lookLimit = 45f;

    private Rigidbody r;

    private float pitch;
    private float yaw;

    private bool grounded;
    private bool allowLook = true;

    void Awake()
    {
        r = GetComponent<Rigidbody>();
        r.freezeRotation = true;
        r.useGravity = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        yaw = transform.eulerAngles.y;
    }

    void Update()
    {
        if (!allowLook || cameraPivot == null) return;

        Vector3 up = transform.up;

        // Mouse input
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = -Input.GetAxis("Mouse Y") * lookSpeed;

        // Keyboard rotation (planet-relative)
        float turn = 0f;

        if (Input.GetKey(KeyCode.LeftArrow))
            turn -= keyboardTurnSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.RightArrow))
            turn += keyboardTurnSpeed * Time.deltaTime;

        transform.Rotate(up, turn, Space.World);

        // Camera rotation
        yaw += mouseX;
        pitch += mouseY;

        pitch = Mathf.Clamp(pitch, -lookLimit, lookLimit);
        yaw = Mathf.Clamp(yaw, -lookLimit, lookLimit);

        // Build stable camera basis using planet up
        Quaternion yawRot = Quaternion.AngleAxis(yaw, up);

        Vector3 forward = yawRot * transform.forward;
        forward = Vector3.ProjectOnPlane(forward, up).normalized;

        Vector3 right = Vector3.Cross(up, forward);

        Quaternion lookRot = Quaternion.LookRotation(forward, up);
        Quaternion pitchRot = Quaternion.AngleAxis(pitch, right);

        cameraPivot.rotation = lookRot * pitchRot;
    }

    void FixedUpdate()
    {
        if (cameraPivot == null) return;

        Vector3 up = transform.up;

        Vector3 forward =
            Vector3.ProjectOnPlane(transform.forward, up).normalized;

        Vector3 right =
            Vector3.ProjectOnPlane(transform.right, up).normalized;

        Vector3 move =
            (forward * Input.GetAxis("Vertical") +
             right * Input.GetAxis("Horizontal")) * speed;

        Vector3 velocity = r.velocity;

        Vector3 velocityChange =
            move - Vector3.ProjectOnPlane(velocity, up);

        velocityChange =
            Vector3.ClampMagnitude(velocityChange, maxVelocityChange);

        r.AddForce(velocityChange, ForceMode.VelocityChange);

        if (grounded && canJump && Input.GetButton("Jump"))
        {
            r.AddForce(up * jumpHeight, ForceMode.VelocityChange);
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