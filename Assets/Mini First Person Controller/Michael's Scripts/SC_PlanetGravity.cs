using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SC_PlanetGravity : MonoBehaviour
{
    public Transform planet;
    public bool alignToPlanet = true;
    
    [Tooltip("The constant acceleration pulling you down toward the planet.")]
    public float gravityConstant = 25f;
    
    [Tooltip("How fast the player aligns their feet to the ground.")]
    public float rotationSpeed = 15f;

    private Rigidbody r;

    void Start()
    {
        r = GetComponent<Rigidbody>();
        
        // Ensure Unity's default workspace gravity doesn't pull the player globally downward
        r.useGravity = false;
    }

    void FixedUpdate()
    {
        if (planet == null) return;

        // Force gravity off every physics step to prevent other scripts from turning it back on
        r.useGravity = false;

        // Calculate direction to the core of the planet
        Vector3 toCenter = planet.position - transform.position;
        float distance = toCenter.magnitude;
        Vector3 gravityDirection = toCenter.normalized;

        // Apply a pure downward acceleration force toward the center core
        r.AddForce(gravityDirection * gravityConstant, ForceMode.Acceleration);

        // Handle structural planetary alignment
        if (alignToPlanet)
        {
            // Set local Up away from the center of the planet
            Vector3 localUp = -gravityDirection;
            
            // Calculate a target rotation where our head points up and we match current forward facing direction
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, localUp) * transform.rotation;
            
            // Smoothly rotate the body to align with the surface curvature
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }
}