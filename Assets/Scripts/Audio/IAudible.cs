using System;
using Cards.Factory;
using Managers;

namespace Audio
{
    public interface IAudible<T> where T : Enum 
    {
        SoundType GetType();

        public void PlayOneShot(T enumVal)
        {
            AudioManager.PlayOneShot(GetType(), enumVal.IntValue());
        }
    }
}