using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(SpriteRenderer), typeof(PixelOutline))]
public class InteractableLight : MonoBehaviour
{
    [SerializeField] Transform _playerTransform;
    [SerializeField] Sprite _onSprite, _offSprite;
    private Light2D _light;
    private SpriteRenderer _spriteRenderer;
    void Start()
    {
        _light = GetComponentInChildren<Light2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = _light.enabled ? _onSprite : _offSprite;
    }
    void Update()
    {
        if (Vector3.Distance(transform.position, _playerTransform.position) < Consts.DistanceDelta && Input.GetKeyUp(KeyCode.E))
        {
            _light.enabled = !_light.enabled;
            _spriteRenderer.sprite = _light.enabled ? _onSprite : _offSprite;
        }
    }
}