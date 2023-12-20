using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PixelOutline : MonoBehaviour
{
    [SerializeField] Transform _playerTransform;
    SpriteRenderer _spriteRenderer;
    Material _outlineMaterial;

    void OnEnable()
    {
        MovementScheduler.OnUpdateMove += OutlineDisplayCheck;
    }
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _outlineMaterial = _spriteRenderer.material;
        OutlineDisplayCheck();
    }

    public void OutlineDisplayCheck()
    {
        bool displayOutline = Vector3.Distance(_playerTransform.position, transform.position) < Consts.DistanceDelta;
        _outlineMaterial.SetFloat("_DisplayOutline", displayOutline ? 1f : 0f);
    }
    void OnDisable()
    {
        MovementScheduler.OnUpdateMove -= OutlineDisplayCheck;
    }
}
