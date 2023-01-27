using Audio;

namespace Player
{
    public partial class PlayerController : IAudible<PlayerSounds>
    {
        
        public SoundType GetSoundType()
        {
            return SoundType.PLAYER;
        }
    }
}