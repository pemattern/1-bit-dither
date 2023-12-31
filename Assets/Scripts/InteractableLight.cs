using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(SpriteRenderer))]
public class InteractableLight : MonoBehaviour, IInteractable
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
    async void Update()
    {
        if (Interaction.CanInteract(transform.position) && Input.GetKeyUp(KeyCode.E))
        {
            CommandScheduler.Add(new Interaction(this));
            await CommandScheduler.Execute();
        }
    }

    public async Task Interact()
    {
        _light.enabled = !_light.enabled;
        _spriteRenderer.sprite = _light.enabled ? _onSprite : _offSprite;
        await Awaitable.WaitForSecondsAsync(Consts.LerpDuration);       
    }

    public async Task UndoInteract()
    {
        await Interact();
    }
}
