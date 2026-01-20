using UnityEngine;

namespace Main.Gameplay
{
    public class FinishLineTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (!GameManager.Instance.isGameActive) return;

            if (other.CompareTag("Player") || other.CompareTag("NPC"))
            {
                GameManager.Instance.OnFinishLineCrossed(other.gameObject);
            }
        }
    }
}
