using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioMixer mixer;
    [SerializeField]
    private BackgroundAudioOptions[] audioOptions;

    public static AudioManager Inst;
    private AudioEvent _currentClip;

    private void Awake() {
        if (Inst == null) {
            Inst = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Inst != this) {
            Destroy(gameObject);
            return;
        }

        _currentClip = AudioEvent.None;
    }

    private void Start() {
        foreach (BackgroundAudioOptions s in audioOptions) {
            mixer.SetFloat(s.volumeControl, -80f);
            s.source.Stop();
        }
    }

    public void ChangeAudio(AudioEvent to, float duration) {
        if (_currentClip == to) return;
        
        if (_currentClip != AudioEvent.None) {
            StartCoroutine(StartFade(mixer, audioOptions[(int) _currentClip - 1].volumeControl, duration, 0f));
            audioOptions[(int) to - 1].source.Stop();
            audioOptions[(int) to - 1].source.Play();
            StartCoroutine(StartFade(mixer, audioOptions[(int) to - 1].volumeControl, duration,
                                     audioOptions[(int) to - 1].volume));
            _currentClip = to;
            return;
        }

        mixer.SetFloat(audioOptions[(int) to - 1].volumeControl, Mathf.Log10(audioOptions[(int) to - 1].volume) * 20f);
        audioOptions[(int) to - 1].source.Stop();
        audioOptions[(int) to - 1].source.Play();
        _currentClip = to;
    }

    private IEnumerator StartFade(AudioMixer audioMixer, string exposedParam, float duration, float targetVolume) {
        float currentTime = 0f;
        float currentVol;
        audioMixer.GetFloat(exposedParam, out currentVol);
        currentVol = Mathf.Pow(10f, currentVol / 20);
        float targetValue = Mathf.Clamp(targetVolume, 0.0001f, 1f);

        while (currentTime < duration) {
            currentTime += Time.deltaTime;
            float newVol = Mathf.Lerp(currentVol, targetValue, currentTime / duration);
            audioMixer.SetFloat(exposedParam, Mathf.Log10(newVol) * 20f);
            yield return null;
        }
    }
}

public enum AudioEvent
{
    None,
    MainMenu,
    Story,
    InGame,
}

[Serializable]
public class BackgroundAudioOptions
{
    public string volumeControl;
    [PropertyRange(0.0001f, 1f)]
    public float volume;
    public AudioSource source;
}