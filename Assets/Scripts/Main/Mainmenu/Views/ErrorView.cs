using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Main.Mainmenu
{
    public class ErrorView : View
    {
        [SerializeField] private TMP_Text errorTitleText;
        [SerializeField] private TMP_Text errorMessageText;

        public void ErrorSetup(string errorTitle, string errorMessage)
        {
            errorTitleText.text = errorTitle;
            errorMessageText.text = errorMessage;
        }
    }
}