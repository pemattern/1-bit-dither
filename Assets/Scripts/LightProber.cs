using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightProber : MonoBehaviour
{
    public static Light2D[] GetAllLights()
    {
        return FindObjectsByType<Light2D>(FindObjectsSortMode.None);
    }

    public static Light2D[] GetContributingLights(Vector3 position)
    {
        List<Light2D> result = new List<Light2D>();
        foreach(Light2D light in GetAllLights())
        {
            Vector3 direction = light.transform.position - position;
            RaycastHit2D hit = Physics2D.Raycast(position, direction.normalized, direction.magnitude, LayerMask.GetMask("Default"));
            if (hit.collider == null && light.enabled && light.lightType == Light2D.LightType.Point && Vector3.Distance(position, light.transform.position) <= light.pointLightOuterRadius)
            {
                result.Add(light);
            }
        }
        return result.ToArray();
    }

    public static float TotalIntensityAt(Vector3 position)
    {
        float intensity = 0f;
        foreach(Light2D light in GetContributingLights(position))
        {
            intensity += IntensityAt(light, position);
        }
        return intensity;
    }

    public static float IntensityAt(Light2D light, Vector3 position)
    {
        float distance = Vector3.Distance(position, light.transform.position);
        if (distance > light.pointLightInnerRadius)
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
