using System.Threading.Tasks;
using UnityEngine;

public class Interaction : ICommand
{
    private IInteractable _interactable;
    public Interaction (IInteractable interactable)
    {
        _interactable = interactable;
    }

    public async Task Do()
    {
        _interactable.Interact();
        await Awaitable.WaitForSecondsAsync(0.1f);
    }
    public async Task Undo()
    {
        _interactable.Interact();
        await Awaitable.WaitForSecondsAsync(0.1f);
    }
}