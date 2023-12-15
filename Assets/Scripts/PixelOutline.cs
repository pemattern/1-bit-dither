using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PixelOutline : MonoBehaviour
{
    [SerializeField] Transform _playerTransform;
    SpriteRenderer _spriteRenderer;
    Material _outlineMaterial;
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _outlineMaterial = _spriteRenderer.material;
    }

    void Update()
    {
        bool displayOutline = Vector3.Distance(transform.position, _playerTransform.position) < 1.1f;
        _outlineMaterial.SetFloat("_DisplayOutline", displayOutline ? 1f : 0f);
    }
}
