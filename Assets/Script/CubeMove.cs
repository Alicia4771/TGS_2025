using UnityEngine;

public class CubeMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        //Debug.Log("Horizontal: " + h);
    }

    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal"); // ← → または A D
        float v = Input.GetAxis("Vertical");   // ↑ ↓ または W S

        Vector3 move = new Vector3(h, 0, v) * moveSpeed;
        rb.linearVelocity = move;
    }
}

