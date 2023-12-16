using UnityEngine;
public static class Lerper
{
    const float LERP_SPEED = 8;

    public static async Awaitable MoveTo(GameObject gameObject, Vector3 direction)
    {
        Vector3 target = gameObject.transform.position + direction;
        while(Vector3.Distance(gameObject.transform.position, target) > 0.05f)
        {
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, target, Time.deltaTime * LERP_SPEED);
            await Awaitable.NextFrameAsync();
        }
        gameObject.transform.position = new Vector3(Mathf.RoundToInt(target.x), Mathf.RoundToInt(target.y), 0f);
    }
}