using UnityEngine;

public class FinishLineTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("NPC"))
        {
            // Kirim GameObject yang menabrak (other.gameObject)
            GameManager.Instance.OnFinishLineCrossed(other.gameObject);
        }
    }
}