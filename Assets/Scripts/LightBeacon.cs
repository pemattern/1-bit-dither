using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class LightBeacon : MonoBehaviour, IActivator
{
    private Collider2D _collider;

    void Start()
    {
        _collider = gameObject.GetComponent<CircleCollider2D>();
    }
    public bool IsActivated()
    {
        return LightProber.TotalIntensityAt(_collider) > Consts.LightActivationIntensity;
    }
}
