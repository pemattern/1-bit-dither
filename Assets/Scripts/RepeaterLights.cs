using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RepeaterLights : MonoBehaviour
{
    private Dictionary<Light2D, Light2D> _lights;
    void OnEnable()
    {
        //CommandScheduler.OnUpdateCommand += UpdateLights;
    }

    void RegisterLight(Light2D light)
    {
        if (_lights.ContainsKey(light)) return;

        GameObject gameObject = new GameObject(light.name + " repeater");
        gameObject.layer = LayerMask.NameToLayer("Light");
        gameObject.transform.parent = transform;
        gameObject.transform.localPosition = Vector3.zero;
        Light2D repeaterLight = gameObject.AddComponent<Light2D>();
        repeaterLight.intensity = 1;
        repeaterLight.pointLightInnerRadius = 5;
        repeaterLight.pointLightOuterRadius = 6;
        repeaterLight.pointLightInnerAngle = 25f;
        repeaterLight.pointLightOuterAngle = 25f;
        repeaterLight.blendStyleIndex = 1;
        repeaterLight.shadowsEnabled = true;
        repeaterLight.shadowIntensity = 1f;
        repeaterLight.falloffIntensity = 0.5f;
        repeaterLight.enabled = false;
        _lights.Add(light, repeaterLight);
    }
    void Start()
    {
        _lights = new Dictionary<Light2D, Light2D>();
    }

    void UpdateLights()
    {
        // foreach(Light2D light in _lights.Values)
        // {
        //     light.enabled = false;
        // }

        // foreach(Light2D light in LightProber.GetContributingLights(transform.position))
        // {           
        //     if (!_lights.ContainsKey(light))
        //         RegisterLight(light);

        //     Vector3 direction = transform.position - light.transform.position;
        //     float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //     Quaternion rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        //     Light2D repeaterLight = _lights[light];
        //     repeaterLight.transform.rotation = rotation;
        //     repeaterLight.intensity = LightProber.IntensityAt(light, transform.position);
        //     repeaterLight.enabled = true;
        // }
    }

    void OnDisable()
    {
        //CommandScheduler.OnUpdateCommand -= UpdateLights;
    }
}
