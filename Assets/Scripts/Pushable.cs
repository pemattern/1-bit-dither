using System.Linq;
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
            CommandScheduler.Add(new Movement(_parent.transform, direction));
            return true;
        }
        if (colliders[0].TryGetComponent(out Pushable other))
        {
            if (other.TryPush(direction))
            {
                CommandScheduler.Add(new Movement(_parent.transform, direction));
                return true;
            }
            return false;
        }
        return false;
    }

    public static bool TryGetAt(Vector3 position, out Pushable pushable)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, Consts.OverlapCircleRadius);
        pushable = null;        
        foreach (Collider2D collider in colliders)
        {
            if (collider.TryGetComponent(out pushable))
            {
                break;
            }
        }
        return pushable != null;
    }
}