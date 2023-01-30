using Audio;
using Managers;

namespace Interactables
{
    public class TreasureInteractable : Interactable
    {
        protected override void OnInteract()
        {
            AudioManager.PlayOneShot(SoundType.SFX, (int)SfxSounds.CHEST);
            GameManager.ShowCards();
        }
    }
}