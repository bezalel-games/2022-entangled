using Audio;
using Managers;

namespace Interactables
{
    public class FountainInteractable : Interactable
    {
        protected override void OnInteract()
        {
            AudioManager.PlayOneShot(SoundType.SFX, (int)SfxSounds.FOUNTAIN);
            var player = GameManager.PlayerController;
            player.OnHeal(player.MaxHp);
        }
    }
}