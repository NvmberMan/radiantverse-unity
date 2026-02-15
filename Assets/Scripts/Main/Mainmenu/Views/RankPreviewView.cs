using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Main.Mainmenu
{
    public class RankPreviewView : View
    {
        public TMP_Text rankView;

        public void UpdatePreview(int rank)
        {
            rankView.text = rank.ToString();
        }
    }
}