using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Main.Gameplay
{
    public class PostProcessController : MonoBehaviour
    {
        public static PostProcessController Instance;

        [SerializeField] private Volume volume;

        private ColorAdjustments color;
        private Bloom bloom;
        private Vignette vignette;
        private ChromaticAberration chromatic;

        private void Awake()
        {
            Instance = this;

            volume.profile.TryGet(out color);
            volume.profile.TryGet(out bloom);
            volume.profile.TryGet(out vignette);
            volume.profile.TryGet(out chromatic);

            DisableEffect();
        }

        public void EnableEffect(Color effectColor, float chromaticIntensity)
        {
            color.postExposure.value = 0.35f;
            bloom.intensity.value = 1f;
            vignette.color.value = effectColor;
            vignette.intensity.value = 0.65f;
            chromatic.intensity.value = chromaticIntensity;
        }

        public void DisableEffect()
        {
            color.postExposure.value = 0f;
            bloom.intensity.value = 0f;
            vignette.intensity.value = 0f;
            chromatic.intensity.value = 0f;
        }
    }
}
