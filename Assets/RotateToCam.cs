using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToCam : MonoBehaviour
{
    // LateUpdate prevents UI jittering by running after the camera moves
    void LateUpdate()
    {
        if (Camera.main != null)
        {
            // 1. Get the camera's current rotation angles
            Vector3 cameraEulerAngles = Camera.main.transform.eulerAngles;

            // 2. Lock the X (tilt) and Z (roll) rotations to 0. 
            // We only keep the Y angle so it rotates horizontally with the screen.
            Quaternion targetHorizontalRotation = Quaternion.Euler(0f, cameraEulerAngles.y, 0f);

            // 3. Apply the clean, upright rotation to the canvas
            transform.rotation = targetHorizontalRotation;
        }
    }
}