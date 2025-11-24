using UnityEngine;
using System.Collections;

public class CharacterItem : CharacterSystem
{
    [Header("Trigger Settings")]
    [SerializeField] private float itemDetectorRange = 1f;
    [SerializeField] private Vector3 itemDetectorOffset = Vector3.zero;
    [SerializeField] private LayerMask itemDetectorLayer;

    [Header("Gizmos Settings")]
    [SerializeField] private Color gizmosDetectorColor = Color.yellow;

    private float originalSpeed;
    private bool isUsingItem = false;


    // ========== SESUAI CLASS DIAGRAM ==========
    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        originalSpeed = CharacterMovement.Acceleration;
    }

    // ========== SESUAI CLASS DIAGRAM (OnEquippedItem) ==========
    private void OnTriggerEnter(Collider other)
    {
        ItemPickup pickup = other.GetComponent<ItemPickup>();

        if (pickup != null)
        {
            Debug.Log("Dapet Item");
            Animator coinAnim = other.GetComponent<Animator>();
            coinAnim.SetBool("Picked", true);


            OnEquippedItem(pickup.itemData);
            Destroy(other.gameObject, 2);
        }
    }

    // Class Diagram memiliki OnEquippedItem()


    public void OnEquippedItem(MovementItemData data)
    {
        if (isUsingItem) return;

        StartCoroutine(ApplyItem(data));
    }

    // ========== Apply Effect ==========
    private IEnumerator ApplyItem(MovementItemData data)
    {
        isUsingItem = true;

        float boostedSpeed = originalSpeed + data.speedBonus;
        CharacterMovement.Acceleration = boostedSpeed;

        Debug.Log($"[ITEM] {data.itemName} applied → speed: {boostedSpeed}");

        yield return new WaitForSeconds(data.duration);

        CharacterMovement.Acceleration = originalSpeed;
        isUsingItem = false;

        Debug.Log($"[ITEM] {data.itemName} ended → speed restored: {originalSpeed}");
    }

    // ========== Gizmos, SESUAI CLASS DIAGRAM ==========
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = gizmosDetectorColor;
        Gizmos.DrawWireSphere(
            transform.position + itemDetectorOffset,
            itemDetectorRange
        );
    }
}
