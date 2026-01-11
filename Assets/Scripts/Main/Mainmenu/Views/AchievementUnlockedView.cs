using Google.Protobuf.WellKnownTypes;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Main.Mainmenu
{
    public class AchievementUnlockedView : View
    {
        public Image previewImage;
        [HideInInspector] public Animator animator;

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        public void Init(AchievementItem unloackedAchievement, float duration = 2f)
        {
            if(unloackedAchievement != null)
                previewImage.sprite = unloackedAchievement.iconPreview;

            if (duration > 0)
            {
                StartCoroutine(AutoClose(duration));
            }
        }

        IEnumerator AutoClose(float duration)
        {
            yield return new WaitForSeconds(duration);
            animator.SetTrigger("Close");
            yield return new WaitForSeconds(1f);
        }
    }
}