using UnityEngine;

public class FinishLineTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Pengecekan Player atau Bot
        if (other.CompareTag("Player") || other.CompareTag("NPC"))
        {
            GameManager.Instance.OnFinishLineCrossed(other.tag);
        }
    }
}