using UnityEngine;

public class LightBeacon : MonoBehaviour, IActivator
{
    private Collider2D _collider;
    void Start()
    {
        _collider = gameObject.GetComponentInChildren<CircleCollider2D>();
    }
    public bool IsActivated()
    {
        bool isActivated = LightProber.TotalIntensityAt(_collider) > Consts.LightActivationIntensity;
        return isActivated;
    }
}
