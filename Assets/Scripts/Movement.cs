using System.Threading.Tasks;
using UnityEngine;

public class Movement : ICommand
{
    private Transform _transform;
    private Vector3 _direction;
    private bool _onlySetSprite;
    private Sprite _previousSprite;
    public Movement (Transform transform, Vector3 direction, bool onlySetSprite = false)
    {
        _transform = transform;
        _direction = direction;
        _onlySetSprite = onlySetSprite;
    }

    public async Task Do()
    {
        SetSprite(Direction.AsFourWay(_direction));
        if (!_onlySetSprite)
            await Lerper.MoveTo(_transform, _direction);
        else 
            await Awaitable.WaitForSecondsAsync(Consts.LerpDuration);
    }
    public async Task Undo()
    {
        if (!_onlySetSprite)
        {
            await Lerper.MoveTo(_transform, -_direction);
            SetSprite(_previousSprite);
        }
        else 
        {
            SetSprite(_previousSprite);
            await Awaitable.WaitForSecondsAsync(Consts.LerpDuration);
        }
    }

    void SetSprite(Direction.FourWay direction)
    {
        if (_transform.TryGetComponent(out DirectionalSprite _directionalSprite))
        {
            _previousSprite = _directionalSprite.GetComponent<SpriteRenderer>().sprite;
            _directionalSprite.SetSprite(direction);
        }
    }

    void SetSprite(Sprite sprite)
    {
        if (_transform.TryGetComponent(out DirectionalSprite _directionalSprite))
        {
            _directionalSprite.GetComponent<SpriteRenderer>().sprite = sprite;
        }
    }
}