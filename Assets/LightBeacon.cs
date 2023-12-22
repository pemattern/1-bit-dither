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
        _lightIntensity = LightProber.GetIntensity(transform.position);
        if (_lightIntensity > Consts.LightActivationIntensity)
        {
            Debug.Log("YOU WIN! " + _lightIntensity);
        }
    }

    void OnDisable()
    {
        MovementScheduler.OnCompletedMove -= UpdateIntensity;
    }
}
