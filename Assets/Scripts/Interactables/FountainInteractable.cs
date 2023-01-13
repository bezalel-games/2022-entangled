using Managers;

namespace Interactables
{
    public class FountainInteractable : Interactable
    {
        protected override void OnInteract()
        {
            var player = GameManager.PlayerController;
            player.OnHeal(player.MaxHp);
        }
    }
}