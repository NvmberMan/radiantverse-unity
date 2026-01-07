using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Main.Mainmenu
{
    public class GameplayGUIView : View
    {
        public TMP_Text rankText;

        public void UpdateRank(int rank, int total)
        {
            rankText.text = $"{rank}/{total}";
        }
    }
}