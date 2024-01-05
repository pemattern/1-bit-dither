using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightProber : MonoBehaviour
{
    public static Light2D[] GetPointLights()
    {
        Light2D[] lights = FindObjectsByType<Light2D>(FindObjectsSortMode.None).Where(light => light.lightType == Light2D.LightType.Point && light.enabled).ToArray();
        return lights;
    }

    public static Light2D[] GetContributingLights(Collider2D collider)
    {
        List<Light2D> result = new List<Light2D>();
        foreach(Light2D light in GetPointLights())
        {
            Vector3 direction;
            if (light.TryGetComponent(out LightChanneler lightChanneler))
            {
                direction = lightChanneler.GetDirection();
            }
            else
            {
                direction = collider.transform.position - light.transform.position;
            }
            RaycastHit2D hit = Physics2D.Raycast(light.transform.position, direction.normalized, light.pointLightOuterRadius, LayerMask.GetMask("Default"));
            if (hit.collider == collider)
            {
                result.Add(light);
            }
        }
        return result.ToArray();
    }

    public static float TotalIntensityAt(Collider2D collider, params Light2D[] lightsToExclude)
    {
        float intensity = 0f;
        foreach(Light2D light in GetContributingLights(collider))
        {
            if (lightsToExclude.Contains(light)) continue;
            intensity += IntensityAt(light, collider);
        }
        return intensity;
    }

    public static float IntensityAt(Light2D light, Collider2D collider)
    {
        float distance = Vector3.Distance(collider.transform.position, light.transform.position);
        if (distance > light.pointLightOuterRadius)
        {
            return 0f;
        }
        else if (distance > light.pointLightInnerRadius)
        {
            float progress = Ease.ProgressNormalized(light.pointLightInnerRadius, light.pointLightOuterRadius, distance);
            float ratio = progress * progress;
            return light.intensity * ratio;
        }
        else
        {
            return light.intensity;
        }
    }
}
