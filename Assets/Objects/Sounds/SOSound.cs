using UnityEngine;
using UnityEngine.Audio;
using System;

[CreateAssetMenu(menuName = "Scriptable Objects/Sound Design/New Sound")]
public class SOSound : ScriptableObject
{
    public Sound[] sounds;
    public MixerType.SoundType soundType;

    public bool isVolumeRandom = false;
    [Range(0f, 1f)]
    public float volume = 0.1f;
    [Range(0f, 1f)]
    public float minRandomVolume;
    [Range(0f, 1f)]
    public float MaxRandomVolume;

    public bool isPitchRandom = false;
    [Range(0f, 3f)]
    public float pitch = 1.0f;
    [Range(0f, 3f)]
    public float minRandomPitch;
    [Range(0f, 3f)]
    public float MaxRandomPitch;

    public bool loop = false;
    /// <summary>
    /// 0 if infinite;
    /// </summary>
    [Min(0)]
    public int numberOfLoops;
}

[Serializable]
public class Sound
{
    public AudioClip clip;
    [Min(0)]
    public float weight = 1;
}