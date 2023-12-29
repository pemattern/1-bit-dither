using UnityEngine;

public class LightBeacon : MonoBehaviour, IActivator
{
    public bool IsActivated()
    {
        Debug.Log(LightProber.TotalIntensityAt(transform.position));
        return LightProber.TotalIntensityAt(transform.position) > Consts.LightActivationIntensity;
    }
}
