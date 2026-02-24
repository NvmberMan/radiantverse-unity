using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Main.Mainmenu
{
    public class UnlockedNewArenaView : View
    {
        public Image arenaPreview;

        public void UpdatePreview(Sprite image)
        {
            arenaPreview.sprite = image;
        }
    }
}