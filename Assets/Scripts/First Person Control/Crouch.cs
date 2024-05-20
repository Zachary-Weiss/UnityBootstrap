using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class Crouch : MonoBehaviour
{
    public float crouchAmount = 0.25f;
    public KeyCode crouchKey = KeyCode.LeftControl;

    private Rigidbody rb;
    private float normalYLocalPosition = 1;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        normalYLocalPosition = rb.transform.localScale.y;
    }


    void Update()
    {

    }
}
