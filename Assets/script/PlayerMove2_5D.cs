using UnityEngine;

public class PlayerMove2_5D : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 7f;
    private Rigidbody rb;
    private Vector3 moveInput;
    private Vector3 moveVelocity;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Input gerak
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        moveInput = new Vector3(moveX, 0f, moveZ).normalized;
        moveVelocity = moveInput * speed;

        // Rotasi tetap
        transform.rotation = Quaternion.Euler(0f, -90f, 0f);

        // Input lompat (hanya kalau di tanah)
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void FixedUpdate()
    {
        transform.Translate(moveVelocity * Time.fixedDeltaTime);
    }

    // Deteksi apakah menyentuh tanah
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
