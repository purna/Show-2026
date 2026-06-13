using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SC_PlanetGravity : MonoBehaviour
{
    public Transform planet;
    public bool alignToPlanet = true;

    public float gravityConstant = 25f;
    public float rotationSpeed = 15f;

    private Rigidbody r;

    void Start()
    {
        r = GetComponent<Rigidbody>();
        r.useGravity = false;
    }

    void FixedUpdate()
    {
        if (planet == null) return;

        r.useGravity = false;

        Vector3 toCenter = planet.position - transform.position;
        Vector3 gravityDir = toCenter.normalized;

        r.AddForce(gravityDir * gravityConstant, ForceMode.Acceleration);

        if (alignToPlanet)
        {
            Vector3 localUp = -gravityDir;

            Quaternion targetRotation =
                Quaternion.FromToRotation(transform.up, localUp) * transform.rotation;

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            );
        }
    }
}