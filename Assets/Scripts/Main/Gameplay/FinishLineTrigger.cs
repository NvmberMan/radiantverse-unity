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
                if (RemoteTestManager.Instance != null && other.CompareTag("Player"))
                {
                    RemoteTestManager.Instance.LogFinish();
                }
                GameManager.Instance.OnFinishLineCrossed(other.gameObject);
                other.GetComponent<CharacterMovement>()._isFreeze = true;
            }
        }
    }
}
