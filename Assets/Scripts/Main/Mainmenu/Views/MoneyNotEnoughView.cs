using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Main.Mainmenu
{
    public class MoneyNotEnoughView : View
    {
        [SerializeField] private TMP_Text messageText;
        [SerializeField] private Button okButton;

        public void Setup(string message, UnityAction okClicked = null)
        {
            messageText.text = message;

            okButton.onClick.RemoveAllListeners();

            if(okClicked != null)
            {
                okClicked?.Invoke();
            }
        }
    }
}