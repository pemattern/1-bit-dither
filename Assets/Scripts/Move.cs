using System.Threading.Tasks;
using UnityEngine;

public class Move : ICommand
{
    private Transform _transform;
    private Vector3 _direction;
    public Move (Transform transform, Vector3 direction)
    {
        _transform = transform;
        _direction = direction;
    }

    public async Task Do()
    {
        await Lerper.MoveTo(_transform, _direction);
    }
    public async Task Undo()
    {
        await Lerper.MoveTo(_transform, -_direction);
    }
}