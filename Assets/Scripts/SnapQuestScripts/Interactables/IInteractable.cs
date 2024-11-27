namespace Code.Interactables
{
    public interface IInteractable
    {
        void SetCurrentInteractable();
        void HandleInteraction();
    }
}