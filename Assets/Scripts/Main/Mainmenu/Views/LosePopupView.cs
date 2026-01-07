using TMPro;

namespace Main.Mainmenu
{
    public class LosePopupView : View
    {
        public TMP_Text expText;
        public TMP_Text arradiusDollarText;

        public void UpdateSummary(int exp, int arradiusDollar)
        {
            expText.text = exp.ToString();
            arradiusDollarText.text = arradiusDollar.ToString();
        }
    }
}