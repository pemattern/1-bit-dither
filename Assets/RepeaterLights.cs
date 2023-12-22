using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RepeaterLights : MonoBehaviour
{
    private Dictionary<Light2D, Light2D> _lights;
    void OnEnable()
    {
        MovementScheduler.OnUpdateMove += UpdateLights;
    }
    void Start()
    {
        _lights = new Dictionary<Light2D, Light2D>();
        Light2D[] lights = LightProber.GetAllLights();
        foreach (Light2D light in lights)
        {
            GameObject gameObject = new GameObject(light.name + " repeater")
            {
                layer = 7
            };
            gameObject.transform.parent = transform;
            gameObject.transform.localPosition = Vector3.zero;
            Light2D repeaterLight = gameObject.AddComponent<Light2D>();
            repeaterLight.intensity = light.intensity;
            repeaterLight.pointLightInnerRadius = light.pointLightInnerRadius;
            repeaterLight.pointLightOuterRadius = light.pointLightOuterRadius;
            repeaterLight.pointLightInnerAngle = 50f;
            repeaterLight.pointLightOuterAngle = 50f;
            repeaterLight.blendStyleIndex = 1;
            _lights.Add(light, repeaterLight);
        }
        UpdateLights();
    }

    void UpdateLights()
    {
        foreach(Light2D light in _lights.Values)
        {
            light.enabled = false;
        }

        foreach(Light2D light in LightProber.GetContributingLights(transform.position))
        {
            if (!_lights.ContainsKey(light))
                break;
                
            Vector3 direction = transform.position - light.transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
            Light2D repeaterLight = _lights[light];
            repeaterLight.transform.rotation = rotation;
            repeaterLight.intensity = LightProber.IntensityAt(light, transform.position);
            repeaterLight.enabled = true;
        }
    }

    void OnDisable()
    {
        MovementScheduler.OnUpdateMove -= UpdateLights;
    }
}
