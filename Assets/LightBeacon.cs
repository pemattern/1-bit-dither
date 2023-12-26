using UnityEngine;

public class LightBeacon : MonoBehaviour
{
    private float _lightIntensity;
    void OnEnable()
    {
        CommandScheduler.OnCompletedCommand += UpdateIntensity;
    }
    void UpdateIntensity()
    {
        _lightIntensity = LightProber.TotalIntensityAt(transform.position);
        if (_lightIntensity > Consts.LightActivationIntensity)
        {
            Debug.Log("YOU WIN! " + _lightIntensity);
        }
    }

    void OnDisable()
    {
        CommandScheduler.OnCompletedCommand -= UpdateIntensity;
    }
}
