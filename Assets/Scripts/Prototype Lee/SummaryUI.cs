using UnityEngine;
using TMPro;

namespace Main.Gameplay
{
    public class SummaryUI : MonoBehaviour
    {
        [Header("Text UI")]
        public TextMeshProUGUI rankText;
        public TextMeshProUGUI expText;
        public TextMeshProUGUI coinText;

        public void Show(int rank, int exp, int coin)
        {
            gameObject.SetActive(true);

            rankText.text = $"{GetRankSuffix(rank)}";
            expText.text = $"{exp}";
            coinText.text = $"{coin}";
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        private string GetRankSuffix(int rank)
        {
            if (rank % 100 >= 11 && rank % 100 <= 13)
                return $"{rank}th";

            switch (rank % 10)
            {
                case 1: return $"{rank}st";
                case 2: return $"{rank}nd";
                case 3: return $"{rank}rd";
                default: return $"{rank}th";
            }
        }
    }
}
