using UnityEngine;

public class PlayerInput : MonoBehaviour, ICharacterInput
{
    CharacterMovement CharacterMovement;

    void Awake()
    {
        CharacterMovement = GetComponent<CharacterMovement>();
    }

    private void Update()
    {
        HandleMovementInput();
        HandleJumpInput();
    }

    public void HandleMovementInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        //Debug.Log("Input H:" + h + "  V:" + v);  // ← Tambahkan ini

        Vector3 dir = new Vector3(h, 0, v).normalized;
        CharacterMovement.MoveToDir(dir);
    }

    public void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CharacterMovement.Jump();
        }
    }
}
