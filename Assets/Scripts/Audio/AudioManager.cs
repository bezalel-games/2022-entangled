using System;
using System.Collections.Generic;
using Audio;
using FMOD.Studio;
using UnityEngine;
using FMODUnity;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class AudioManager : MonoBehaviour
{
    #region Serialized Fields

    [SerializeField] private AudioBank _bank;

    #endregion

    #region Non-Serialized Fields

    private static AudioManager _instance;

    private List<EventInstance> _instances = new();
    private EventInstance _musicEventInstance;
        
    private static readonly string _musicEventName = "Section";
        
    #endregion

    #region Properties

    #endregion

    #region Function Events

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(_instance.gameObject);
        InitializeMusicEventInstance(_bank.MusicEventReference);
    }
    
    private void OnDestroy()
    {
        CleanUp();
    }

    #endregion

    #region Public Methods

    public static void PlayOneShot(SoundType type, int val)
    {
        try
        {
            RuntimeManager.PlayOneShot(_instance._bank[type, val]);
        }
        catch (ArgumentOutOfRangeException)
        {
            print("Invalid sound");
        }
    }

    public static void SetMusic(MusicSounds music)
    {
        print($"Set music to {music}");
        _instance._musicEventInstance.setParameterByName(_musicEventName, (float) music);
    }

    #endregion

    #region Private Methods

    private void InitializeMusicEventInstance(EventReference reference)
    {
        _musicEventInstance = CreateEventInstance(reference);
        _musicEventInstance.start();
    }

    private EventInstance CreateEventInstance(EventReference reference)
    {
        EventInstance instance = RuntimeManager.CreateInstance(reference);
        _instances.Add(instance);
        return instance;
    }
    
    private void CleanUp()
    {
        foreach (var instance in _instances)
        {
            instance.stop(STOP_MODE.IMMEDIATE);
            instance.release();
        }
    }
    
    #endregion
}