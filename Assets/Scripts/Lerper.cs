using System;
using System.Threading.Tasks;
using UnityEngine;
public class Lerper : MonoBehaviour
{
    private static Lerper _instance;

    void Awake() { _instance = this; }

    public static async Task MoveTo(Transform transform, Vector3 direction)
    {
        Vector3 target = transform.position + direction;
        while(Vector3.Distance(transform.position, target) > Consts.DistanceDelta - 1)
        {
            transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * Consts.LerpSpeed);
            await Awaitable.NextFrameAsync();
        }
        transform.position = new Vector3(Mathf.RoundToInt(target.x), Mathf.RoundToInt(target.y), 0f);
    }
}