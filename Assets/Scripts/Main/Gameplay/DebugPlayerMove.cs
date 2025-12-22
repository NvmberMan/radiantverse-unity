using UnityEngine;

public class DebugPlayerMove : MonoBehaviour
{
    [Header("Dummy Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // (WASD / Arrow Keys)
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // (Pergerakan di bidang X, Y, Z)
        Vector3 movement = new Vector3(h, 0, v) * moveSpeed * Time.deltaTime;
        transform.Translate(movement, Space.World);

        // Lompat
        if (Input.GetKeyDown(KeyCode.Space) && rb != null)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}