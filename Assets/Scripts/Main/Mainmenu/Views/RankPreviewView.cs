using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Main.Mainmenu
{
    public class RankPreviewView : View
    {
        public Image rankImagePreview;

        public void UpdatePreview(Sprite image)
        {
            rankImagePreview.sprite = image;
        }
    }
}