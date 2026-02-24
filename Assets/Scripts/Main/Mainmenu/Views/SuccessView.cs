using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Main.Mainmenu
{
    public class SuccessView : View
    {
        [SerializeField] private TMP_Text successTitleText;
        [SerializeField] private TMP_Text successMessageText;
        [SerializeField] private Button okButton;
        public void SuccessSetup(string successTitle, string successMessage, UnityAction onOkClick = null)
        {
            successTitleText.text = successTitle;
            successMessageText.text = successMessage;

            okButton.onClick.RemoveAllListeners();

            if(onOkClick != null)
            {
                okButton.onClick.AddListener(onOkClick);
            }
        }
    }
}