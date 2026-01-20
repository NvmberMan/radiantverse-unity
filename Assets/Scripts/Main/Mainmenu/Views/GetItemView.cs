using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Main.Mainmenu
{
    public class GetItemView : View
    {
        public Image previewImage;
        [HideInInspector] public Animator animator;

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        public void Init(AccessoryData item, float duration = 2f)
        {
            previewImage.sprite = item.icon;

            if (duration > 0)
            {
                StartCoroutine(AutoClose(duration));
            }
        }

        IEnumerator AutoClose(float duration)
        {
            yield return new WaitForSeconds(duration);
            animator.SetTrigger("Close");
        }
    }
}