using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightBeacon : MonoBehaviour
{
    private float _lightIntensity;
    void OnEnable()
    {
        MovementScheduler.OnCompletedMove += UpdateIntensity;
    }
    void UpdateIntensity()
    {
        _lightIntensity = LightProber.TotalIntensityAt(transform.position);
        if (_lightIntensity > Consts.LightActivationIntensity)
        {
            //
        }
    }

    void OnDisable()
    {
        MovementScheduler.OnCompletedMove -= UpdateIntensity;
    }
}
