using System;
using Cards.Factory;
using FMOD.Studio;
using Managers;

namespace Audio
{
    public interface IAudible<T> where T : Enum 
    {
        SoundType GetSoundType();

        public void PlayOneShot(T enumVal)
        {
            AudioManager.PlayOneShot(GetSoundType(), enumVal.IntValue());
        }
        
        public EventInstance CreateEventInstance(T enumVal)
        {
            return AudioManager.CreateEventInstance(GetSoundType(), enumVal.IntValue());
        }
    }
}