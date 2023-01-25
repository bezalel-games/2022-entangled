using Managers;

namespace Interactables
{
    public class TreasureInteractable : Interactable
    {
        protected override void OnInteract()
        {
            GameManager.ShowCards();
        }
    }
}