using UnityEngine;

public class Pushable : MonoBehaviour
{
    [SerializeField] GameObject _parent;

    public bool TryPush(Vector3 direction)
    {
        Vector3 target = transform.position + direction;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(target, Consts.OverlapCircleRadius);
        if (colliders.Length == 0)
        {
            CommandScheduler.Add(new Move(_parent.transform, direction));
            return true;
        }
        if (colliders[0].TryGetComponent(out Pushable other))
        {
            if (other.TryPush(direction))
            {
                CommandScheduler.Add(new Move(_parent.transform, direction));
                return true;
            }
            return false;
        }
        return false;
    }

    public static bool TryGetAt(Vector3 position, out Pushable pushable)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, Consts.OverlapCircleRadius);
        if (colliders.Length > 0 && colliders[0].TryGetComponent(out pushable))
            return true;
        pushable = null;
        return false;
    }
}