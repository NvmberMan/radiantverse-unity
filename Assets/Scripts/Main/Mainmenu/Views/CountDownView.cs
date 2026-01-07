using TMPro;

namespace Main.Mainmenu
{
    public class CountDownView : View
    {
        public TMP_Text countText;

        public void UpdateText(string value)
        {
            countText.text = value;
        }
    }
}