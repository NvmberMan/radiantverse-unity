using Cinemachine;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraShakeManager : MonoBehaviour
{
    [Header("Virtual Camera")]
    public CinemachineVirtualCamera vcam; // drag Virtual Camera di Inspector
    private CinemachineBasicMultiChannelPerlin noise;

    void Awake()
    {
        if (vcam == null)
            vcam = GetComponent<CinemachineVirtualCamera>();

        noise = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        if (noise == null)
        {
            Debug.LogWarning("Noise profile missing! Add CinemachineBasicMultiChannelPerlin to Virtual Camera.");
        }
    }

    public void Shake(float duration, float amplitude)
    {
        StartCoroutine(ShakeRoutine(duration, amplitude));
    }

    private IEnumerator ShakeRoutine(float duration, float amplitude)
    {
        if (noise == null) yield break;

        noise.AmplitudeGain = amplitude;
        yield return new WaitForSeconds(duration);
        noise.AmplitudeGain = 0f;
    }
}
