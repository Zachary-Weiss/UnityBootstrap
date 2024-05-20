using UnityEngine;

[RequireComponent(typeof(Collider),typeof(Rigidbody))]
public class Move : MonoBehaviour
{
    public float walkSpeed = 5;
    public float runSpeed = 10;
    public KeyCode runKey = KeyCode.LeftShift;
    public float drag;
    public float gravity;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float speed = Input.GetKey(runKey) ? runSpeed : walkSpeed;

        float inputX = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        float inputZ = Input.GetAxis("Vertical") * speed * Time.deltaTime;

        rb.transform.Translate(inputX, 0, inputZ);
        //movement isn't based on velocity, so add a drag factor decreasing velocity

    }
    private void FixedUpdate()
    {
        Gravity();
    }
    private void Gravity()
    {
        rb.AddForce(Vector3.down * gravity * rb.mass);
    }
}
