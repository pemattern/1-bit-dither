using System.Threading.Tasks;

public interface IInteractable
{
    public Task Interact();
    public Task UndoInteract();
}