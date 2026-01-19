using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class PostProcessController : MonoBehaviour
{
    public static PostProcessController Instance;

    [SerializeField] private Volume volume;

    private ColorAdjustments color;
    private Bloom bloom;
    private Vignette vignette;
    private ChromaticAberration chromatic;

    Coroutine routine;

    void Awake()
    {
        Instance = this;

        volume.profile.TryGet(out color);
        volume.profile.TryGet(out bloom);
        volume.profile.TryGet(out vignette);
        volume.profile.TryGet(out chromatic);

        ResetEffect();
    }

    public void PlayEffect(float duration, Color effectColor)
    {
        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(EffectRoutine(duration, effectColor));
    }

    IEnumerator EffectRoutine(float duration, Color effectColor)
    {
        yield return Lerp(0, 1, 0.25f, effectColor);
        yield return new WaitForSeconds(duration);
        yield return Lerp(1, 0, 0.25f, effectColor);
    }

    IEnumerator Lerp(float from, float to, float time, Color effectColor)
    {
        float t = 0f;
        while (t < time)
        {
            t += Time.deltaTime;
            float v = Mathf.Lerp(from, to, t / time);

            // 🎨 Tengah layar tetap terang
            color.colorFilter.value =
                Color.Lerp(Color.white, effectColor, v * 0.55f);

            // 🔵 VIGNETTE SAMA UNTUK SEMUA ITEM
            vignette.color.value = effectColor;
            vignette.intensity.value = v * 1f;            // FIX
            vignette.smoothness.value = 0.363f;           // FIX
            vignette.center.value = new Vector2(0.5f, 0.55f);

            // ✨ Extra (halus & konsisten)
            bloom.intensity.value = v * 0.4f;
            chromatic.intensity.value = v * 0.15f;

            yield return null;
        }
    }

    void ResetEffect()
    {
        color.colorFilter.value = Color.white;

        vignette.color.value = Color.black;
        vignette.intensity.value = 0f;
        vignette.smoothness.value = 0.363f;
        vignette.center.value = new Vector2(0.5f, 0.55f);

        bloom.intensity.value = 0f;
        chromatic.intensity.value = 0f;
    }
}
