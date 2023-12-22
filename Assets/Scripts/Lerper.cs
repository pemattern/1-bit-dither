using System.Threading.Tasks;
using UnityEngine;
public class Lerper : MonoBehaviour
{
    public static async Task MoveTo(Transform transform, Vector3 direction)
    {
        Vector3 startingPosition = transform.position;
        Vector3 target = startingPosition + direction;

        float duration = 0f;
        while(duration < Consts.LerpDuration)
        {
            float progress = Ease.ProgressNormalized(0f, Consts.LerpDuration, duration);
            transform.position = Vector3.Lerp(target, startingPosition, Ease.Smooth(progress));
            duration += Time.deltaTime;
            await Awaitable.NextFrameAsync();
        }
        transform.position = new Vector3(Mathf.RoundToInt(target.x), Mathf.RoundToInt(target.y), 0f);
    }
}