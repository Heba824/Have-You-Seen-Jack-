public interface IInteractable
{
    string GetInteractPrompt(); // e.g. "Press E to Sleep", "Press E to Pick Up"
    void Interact();
}