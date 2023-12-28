using UnityEngine;

public class LightBeacon : MonoBehaviour, IActivator
{
    public bool IsActivated()
    {
        return LightProber.TotalIntensityAt(transform.position) > Consts.LightActivationIntensity;
    }
}
