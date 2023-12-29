using UnityEngine;

[RequireComponent(typeof(Activatable), typeof(SpriteRenderer), typeof(Collider2D))]
public class Door : MonoBehaviour
{
    [SerializeField] private Sprite _closedSprite, _openSprite;
    private Activatable _activatable;
    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;

    void OnEnable()
    {
        CommandScheduler.OnCompletedCommand += MessageStatus;
    }

    void Start()
    {
        _activatable = GetComponent<Activatable>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
    }

    void MessageStatus()
    {
        bool isActivated = _activatable.IsActivated();
        _spriteRenderer.sprite = isActivated ? _openSprite : _closedSprite;
        _collider.enabled = !isActivated;
    }
    void OnDisable()
    {
        CommandScheduler.OnCompletedCommand -= MessageStatus;
    }
}
