using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BasicAnimation : MonoBehaviour
{
    [SerializeField] private Sprite[] _sprites;
    [SerializeField] private int _framesPerSecond;
    private int _currentSpriteIndex;
    private SpriteRenderer _spriteRenderer;
    private Awaitable _frameTimer;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _frameTimer = Awaitable.WaitForSecondsAsync(1f / _framesPerSecond);
    }

    async void Update()
    {
        while (!_frameTimer.IsCompleted)
            await Awaitable.NextFrameAsync();

        if (_currentSpriteIndex >=  _sprites.Length - 1)
            _currentSpriteIndex = 0;
        else
            _currentSpriteIndex++;

        _spriteRenderer.sprite = _sprites[_currentSpriteIndex];
        _frameTimer = Awaitable.WaitForSecondsAsync(1f / _framesPerSecond);
    }
}
