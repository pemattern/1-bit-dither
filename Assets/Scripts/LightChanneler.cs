using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class LightChanneler : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Direction.FourWay _startingDirection;
    private Direction.FourWay _currentDirection;
    private Light2D _channelerLight, _cosmeticLight;
    private Collider2D _collider;

    void OnEnable()
    {
        CommandScheduler.OnUpdateCommand += UpdateLight;
        CommandScheduler.OnCompletedCommand += UpdateLight;
    }

    void Start()
    {
        _currentDirection = _startingDirection;
        _channelerLight = GetComponent<Light2D>();
        _cosmeticLight = GetComponentInChildren<Light2D>();
        _collider = GetComponentInParent<CircleCollider2D>();
        UpdateLight();
    }

    void UpdateLight()
    {
        float intensity = LightProber.TotalIntensityAt(_collider, _channelerLight, _cosmeticLight);
        bool enabled = intensity > 0f;
        _channelerLight.enabled = enabled;
        _cosmeticLight.enabled = enabled;
        _channelerLight.transform.rotation = GetRotation();
        _channelerLight.intensity = Mathf.Clamp01(intensity);
        _cosmeticLight.intensity = Mathf.Clamp01(intensity);
    }

    public Vector3 GetDirection()
    {
        return Direction.AsVector(_currentDirection);
    }

    Quaternion GetRotation()
    {
        Vector3 direction = Direction.AsVector(_currentDirection);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        return rotation;
    }

    async void Update()
    {
        if (Vector3.Distance(transform.position, _playerTransform.position) < Consts.DistanceDelta && Input.GetKeyUp(KeyCode.E))
        {
            CommandScheduler.Add(new Interaction(this));
            await CommandScheduler.Execute();
        }
    }

    public void Interact()
    {
        _currentDirection = Direction.Rotate(_currentDirection);
    }

    public void UndoInteract()
    {
        _currentDirection = Direction.Rotate(_currentDirection, false);
    }

    void OnDisable()
    {
        CommandScheduler.OnUpdateCommand -= UpdateLight;
        CommandScheduler.OnCompletedCommand -= UpdateLight;
    }
}
