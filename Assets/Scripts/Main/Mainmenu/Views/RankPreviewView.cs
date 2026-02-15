using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Main.Mainmenu
{
    public class RankPreviewView : View
    {
        public TMP_Text rankView;
        public TMP_Text subRankView;

        public void UpdatePreview(int rank)
        {
            rankView.text = rank.ToString();

            if(rank == 1)
            {
                subRankView.text = "st";
            }else if(rank == 2)
            {
                subRankView.text = "nd";
            }else if(rank == 3)
            {
                subRankView.text = "rd";
            }
            else
            {
                subRankView.text = "th";
            }
        }
    }
}