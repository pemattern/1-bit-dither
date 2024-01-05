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
        await _interactable.Interact();
        await Awaitable.WaitForSecondsAsync(Consts.LerpDuration);
    }
    public async Task Undo()
    {
        await _interactable.UndoInteract();
        await Awaitable.WaitForSecondsAsync(Consts.LerpDuration);
    }
}