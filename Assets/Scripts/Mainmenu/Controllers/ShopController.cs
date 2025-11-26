using UnityEngine;

public class ShopController : Controller
{
    [Header("Views")]
    public ArradiusDollarView ArradiusDollarView;

    public override void Show()
    {
        base.Show();

        ArradiusDollarView.Initialization();
    }

    public override void Hide()
    {
        base.Hide();
    }
}
