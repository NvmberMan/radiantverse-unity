using TMPro;

namespace Main.Mainmenu
{
    public class CountDownView : View
    {
        public TMP_Text countText;

        public void UpdateCount(int value)
        {
            countText.text = value.ToString();
        }
    }
}