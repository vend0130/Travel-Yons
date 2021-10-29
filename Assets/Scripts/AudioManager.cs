using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private Sound[] sounds;

    private Sound currentTheme = null;

    private void Awake()
    {
        if (I.audioManager != null)
        {
            Destroy(gameObject);
            return;
        }

        I.audioManager = this;
        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.loop = s.loop;
        }
    }

    public void Play(string name)
    {
        if (!I.sfx && name != "Win" && name != "Loss")
            return;

        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogError("Sound \"" + name + "\" not found(");
            return;
        }

        s.source.Play();
    }

    public void PlayTheme(string name)
    {
        if (!I.music)
            return;

        Sound _sound = Array.Find(sounds, sound => sound.name == name);

        if (_sound == null)
        {
            Debug.LogError("Sound \"" + name + "\" not found(");
            return;
        }

        if (_sound == currentTheme)
            return;

        if (currentTheme != null)
        {
            currentTheme.source.Stop();
        }

        currentTheme = _sound;
        _sound.source.Play();
    }

    public void StopTheme()
    {
        if (currentTheme == null)
            return;
        currentTheme.source.Stop();
        currentTheme = null;
    }

    public void Volume(float _time)
    {
        if (currentTheme == null)
            return;
        currentTheme.source.volume = .05f;
        StartCoroutine(MininmumVolume(currentTheme, _time));

    }

    private IEnumerator MininmumVolume(Sound _sound, float _time)
    {
        yield return new WaitForSeconds(_time);
        _sound.source.volume = _sound.volume;
        yield return null;
    }
}

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume;
    public bool loop;

    [HideInInspector] public AudioSource source;
}
