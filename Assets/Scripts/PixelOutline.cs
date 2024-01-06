using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PixelOutline : MonoBehaviour
{
    public bool Active => Interaction.CanInteract(transform.position);
    [SerializeField] Transform _playerTransform;
    [SerializeField] PixelOutline[] _linkedOutlines;
    SpriteRenderer _spriteRenderer;
    Material _outlineMaterial;
    void OnEnable()
    {
        CommandScheduler.OnCompletedCommand += OutlineDisplayCheck;
    }
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _outlineMaterial = _spriteRenderer.material;
        UpdateOutline(Active);
    }

    public void UpdateOutline(bool displayOutline)
    {
        _outlineMaterial.SetFloat("_DisplayOutline", displayOutline ? 1f : 0f);
    }

    public void OutlineDisplayCheck()
    {
        bool displayOutline = Active;
        if (!displayOutline)
        {
            foreach(PixelOutline outline in _linkedOutlines)
            {
                if (outline.Active)
                {
                    displayOutline = true;
                    break;
                }
            }
        }
        UpdateOutline(displayOutline);
        foreach(PixelOutline outline in _linkedOutlines)
        {
            outline.UpdateOutline(displayOutline);
        }

    }
    void OnDisable()
    {
        CommandScheduler.OnCompletedCommand -= OutlineDisplayCheck;
    }
}
