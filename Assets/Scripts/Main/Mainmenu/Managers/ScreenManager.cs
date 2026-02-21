using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    void Start()
    {
        // 1. Matikan orientasi Portrait (Berdiri)
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;

        // 2. Aktifkan kedua sisi Landscape (Tidur)
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;

        // 3. Set mode ke AutoRotation agar sistem mendeteksi sensor
        Screen.orientation = ScreenOrientation.AutoRotation;
    }
}