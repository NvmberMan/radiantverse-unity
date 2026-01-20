using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Mixer Setup")]
    [SerializeField] private AudioMixer mainMixer;
    [SerializeField] private AudioMixerGroup musicGroup;
    [SerializeField] private AudioMixerGroup sfxGroup;
    [SerializeField] private AudioMixerGroup voiceGroup;
    [SerializeField] private AudioMixerGroup ambienceGroup;

    [Header("Sound Library")]
    [SerializeField] private List<SoundEffect> soundLibrary;
    private Dictionary<string, AudioClip> soundDictionary = new Dictionary<string, AudioClip>();

    private AudioSource musicSource;
    private AudioSource sfxSource;

    public const string MUSIC_KEY = "MusicVol";
    public const string SFX_KEY = "SFXVol";
    public const string VOICE_KEY = "VoiceVol";
    public const string AMBIENCE_KEY = "AmbienceVol";

    private void Awake()
    {
        Instance = this;
        InitializeManager();
    }

    private void InitializeManager()
    {
        foreach (var sound in soundLibrary)
        {
            if (!soundDictionary.ContainsKey(sound.name))
                soundDictionary.Add(sound.name, sound.clip);
        }

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.outputAudioMixerGroup = musicGroup;
        musicSource.loop = true;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.outputAudioMixerGroup = sfxGroup;
    }

    private void Start()
    {
        LoadAllVolume();
    }

    public void PlaySFX(string soundName)
    {
        if (soundDictionary.TryGetValue(soundName, out AudioClip clip))
        {
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("SFX: " + soundName + " tidak ditemukan di Library!");
        }
    }

    public void PlayMusic(string soundName)
    {
        if (soundDictionary.TryGetValue(soundName, out AudioClip clip))
        {
            if (musicSource.clip == clip) return;
            musicSource.clip = clip;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning("Music: " + soundName + " tidak ditemukan di Library!");
        }
    }

    public void StopMusic() => musicSource.Stop();

    public void SetVolume(string key, float value)
    {
        float dB = value > 0.0001f ? Mathf.Log10(value) * 20 : -80f;

        mainMixer.SetFloat(key, dB);
        PlayerPrefs.SetFloat(key, value);
    }

    private void LoadAllVolume()
    {
        SetVolume(MUSIC_KEY, PlayerPrefs.GetFloat(MUSIC_KEY, 0.75f));
        SetVolume(SFX_KEY, PlayerPrefs.GetFloat(SFX_KEY, 0.75f));
        SetVolume(VOICE_KEY, PlayerPrefs.GetFloat(VOICE_KEY, 0.75f));
        SetVolume(AMBIENCE_KEY, PlayerPrefs.GetFloat(AMBIENCE_KEY, 0.75f));
    }
}

[System.Serializable]
public class SoundEffect
{
    public string name;
    public AudioClip clip;
}