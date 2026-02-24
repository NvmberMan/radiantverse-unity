using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Main.Mainmenu
{
    public class WinPopupView : View
    {
        public TMP_Text expText;
        public TMP_Text arradiusDollarText;
        public TMP_Text unloackedArenaText;

        public void UpdateSummary(int exp, int arradiusDollar, string unlockedArena)
        {
            expText.text = exp.ToString();
            arradiusDollarText.text = arradiusDollar.ToString();
            unloackedArenaText.text = unlockedArena;
        }
    }
}