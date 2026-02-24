using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Main.Mainmenu
{
    public class LosePopupView : View
    {
        public TMP_Text expText;
        public TMP_Text arradiusDollarText;
        public TMP_Text rankText;

        public void UpdateSummary(int exp, int arradiusDollar, int rank)
        {
            expText.text = exp.ToString();
            arradiusDollarText.text = arradiusDollar.ToString();
            rankText.text = rank.ToString();
        }
    }
}