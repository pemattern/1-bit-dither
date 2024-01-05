using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightFunnel : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Direction.FourWay _startingDirection;
    private Direction.FourWay _currentDirection;
    private Light2D _light;
    private Collider2D _collider;

    void OnEnable()
    {
        CommandScheduler.OnUpdateCommand += UpdateLight;
        CommandScheduler.OnCompletedCommand += UpdateLight;
    }

    void Start()
    {
        _currentDirection = _startingDirection;

        _light = gameObject.AddComponent<Light2D>();
        _light.pointLightInnerRadius = 5;
        _light.pointLightOuterRadius = 6;
        _light.pointLightInnerAngle = 10f;
        _light.pointLightOuterAngle = 10f;
        _light.blendStyleIndex = 1;
        _light.shadowsEnabled = true;
        _light.shadowIntensity = 1f;
        _light.falloffIntensity = 0.5f;

        _collider = gameObject.transform.parent.gameObject.AddComponent<CircleCollider2D>();

        UpdateLight();
    }

    void UpdateLight()
    {
        float intensity = LightProber.TotalIntensityAt(_collider, _light);
        _light.enabled = intensity > 0f;
        _light.transform.rotation = GetRotation();
        _light.intensity = Mathf.Clamp01(intensity);
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
