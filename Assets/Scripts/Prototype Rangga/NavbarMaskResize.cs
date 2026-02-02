using UnityEngine;

public class NavbarMaskResize : MonoBehaviour
{
    public RectTransform maskArea;
    public float targetWidth;
    public float speed = 10f;

    void Update()
    {
        Vector2 size = maskArea.sizeDelta;
        size.x = Mathf.Lerp(size.x, targetWidth, Time.deltaTime * speed);
        maskArea.sizeDelta = size;
    }

    public void RevealTo(int index, float buttonWidth)
    {
        targetWidth = (index + 1) * buttonWidth;
    }
}
