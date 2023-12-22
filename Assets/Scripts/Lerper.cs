using System;
using System.Threading.Tasks;
using UnityEngine;
public class Lerper : MonoBehaviour
{
    private static Lerper _instance;
    [SerializeField] EaseFunction _easeFunction;

    void Awake() { _instance = this; }
    public static async Task MoveTo(Transform transform, Vector3 direction)
    {
        Vector3 startingPosition = transform.position;
        Vector3 target = startingPosition + direction;

        float duration = 0f;
        while(duration < Consts.LerpDuration)
        {
            float progress = Ease.ProgressNormalized(0f, Consts.LerpDuration, duration);
            transform.position = Vector3.Lerp(target, startingPosition, GetEase(progress));
            duration += Time.deltaTime;
            await Awaitable.NextFrameAsync();
        }
        transform.position = new Vector3(Mathf.RoundToInt(target.x), Mathf.RoundToInt(target.y), 0f);
    }

    private static float GetEase(float progress)
    {
        return _instance._easeFunction switch
        {
            EaseFunction.Linear => Ease.Linear(progress),
            EaseFunction.Smooth => Ease.Smooth(progress),
            EaseFunction.CircleLinear => Ease.CircleInOutLinear(progress),
            EaseFunction.CircleSmooth => Ease.CircleInOutSmooth(progress),
            _ => Ease.Linear(progress),
        };
    }

    enum EaseFunction
    {
        Linear,
        Smooth,
        CircleLinear,
        CircleSmooth,
    }
}