using System.Threading.Tasks;
using UnityEngine;

public class Interact : ICommand
{
    private IInteractable _interactable;
    public Interact (IInteractable interactable)
    {
        _interactable = interactable;
    }

    public async Task Do()
    {
        _interactable.Interact();
    }
    public async Task Undo()
    {
        _interactable.Interact();
    }
}