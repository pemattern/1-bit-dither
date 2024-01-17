using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class LightChanneler : MonoBehaviour, IInteractable
{
    [SerializeField] private Direction.FourWay _startingDirection;
    [SerializeField] private Collider2D _collider;
    private Vector3 _currentDirection;
    private Light2D _channelerLight;
    
    void OnEnable()
    {
        CommandScheduler.OnUpdateCommand += UpdateLights;
        CommandScheduler.OnCompletedCommand += UpdateLights;
    }

    void Start()
    {
        _currentDirection = Direction.AsVector(_startingDirection);
        transform.rotation = GetRotation(_currentDirection);
        _channelerLight = GetComponent<Light2D>();
        UpdateLights();
    }

    void UpdateLights()
    {
        float intensity = LightProber.TotalIntensityAt(_collider, _channelerLight);
        _channelerLight.intensity = Mathf.Clamp01(intensity);

        bool enabled = intensity > 0f;
        _channelerLight.enabled = enabled;
    }

    public Vector3 GetDirection()
    {
        return transform.rotation * Vector3.up;
    }

    Quaternion GetRotation(Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        return rotation;
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
        Vector3 _previousDirection = _currentDirection;
        _currentDirection = Direction.Rotate(_currentDirection);
        Vector3 _targetDirection = _currentDirection;
        
        float duration = 0f;
        while(duration < Consts.LerpDuration)
        {
            float progress = Ease.CircleOut(Ease.ProgressNormalized(0f, Consts.LerpDuration, duration));
            Vector3 direction = Vector3.Lerp(_targetDirection, _previousDirection, progress);
            transform.rotation = GetRotation(direction);
            duration += Time.deltaTime;
            await Awaitable.NextFrameAsync();
        }
        transform.rotation = GetRotation(_targetDirection);
    }

    public async Task UndoInteract()
    {
        Vector3 _previousDirection = _currentDirection;
        _currentDirection = Direction.Rotate(_currentDirection, false);
        Vector3 _targetDirection = _currentDirection;
        
        float duration = 0f;
        while(duration < Consts.LerpDuration)
        {
            float progress = Ease.CircleOut(Ease.ProgressNormalized(0f, Consts.LerpDuration, duration));
            Vector3 direction = Vector3.Lerp(_targetDirection, _previousDirection, progress);
            transform.rotation = GetRotation(direction);
            duration += Time.deltaTime;
            await Awaitable.NextFrameAsync();
        }
        transform.rotation = GetRotation(_targetDirection);
    }

    void OnDisable()
    {
        CommandScheduler.OnUpdateCommand -= UpdateLights;
        CommandScheduler.OnCompletedCommand -= UpdateLights;
    }
}
