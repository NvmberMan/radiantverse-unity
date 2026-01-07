using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Main.Mainmenu
{
    public class LosePopupView : View
    {
        public TMP_Text expText;
        public TMP_Text arradiusDollarText;
        public Image rankImage;

        public void UpdateSummary(int exp, int arradiusDollar, Sprite rankSprite)
        {
            expText.text = exp.ToString();
            arradiusDollarText.text = arradiusDollar.ToString();
            rankImage.sprite = rankSprite;
        }
    }
}