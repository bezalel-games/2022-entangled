using Audio;

namespace Player
{
    public partial class PlayerController : IAudible<PlayerSounds>
    {
        
        public SoundType GetType()
        {
            return SoundType.PLAYER;
        }
    }
}