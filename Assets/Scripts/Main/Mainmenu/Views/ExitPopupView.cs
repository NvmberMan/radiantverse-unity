using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Main.Mainmenu
{
    public class ExitPopupView : View
    {
        [SerializeField] private Button yesButton;
        [SerializeField] private Button noButton;

        void Start ()
        {
            noButton.onClick.AddListener(CancelButton);
            yesButton.onClick.AddListener(ExitGame);
        }

        void CancelButton()
        {
            Hide();
        }

        void ExitGame()
        {
            Application.Quit();
        }
    }
}