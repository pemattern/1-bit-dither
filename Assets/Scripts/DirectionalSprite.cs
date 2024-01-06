using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DirectionalSprite : MonoBehaviour
{
    [SerializeField] private Sprite _upSprite, _downSprite, _leftSprite, _rightSprite;
    private SpriteRenderer _spriteRenderer;
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetSprite(Direction.FourWay direction)
    {
        _spriteRenderer.sprite = direction switch
        {
            Direction.FourWay.Up => _upSprite,
            Direction.FourWay.Down => _downSprite,
            Direction.FourWay.Left => _leftSprite,
            Direction.FourWay.Right => _rightSprite,
            _ => throw new System.Exception("No valid direction given.")
        };
    }
}
