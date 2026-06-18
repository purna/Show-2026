using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovementOnPlanet : MonoBehaviour
{
    private Rigidbody rigid;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    Vector3 planetGravity;

    private void FixedUpdate()
    {
        planetGravity = Quaternion.FromToRotation(transform.up, transform.position) * Physics.gravity;
        rigid.AddForce(planetGravity, ForceMode.Force);
        rigid.MoveRotation(planetGravity.ConvertTo<Quaternion>());
    }
}
