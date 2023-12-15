using UnityEngine;

public class Pushable : MonoBehaviour
{
    [SerializeField] GameObject _parent;
    public bool TryPush(Vector3 direction)
    {
        Vector3 target = transform.position + direction;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(target, 0.25f);//, LayerMask.NameToLayer("Pushable"));
        if (colliders.Length == 0)
        {
            Move(target);
            return true;
        }
        if (colliders.Length > 1)
        {
            return false;
        }
        if (colliders[0].TryGetComponent(out Pushable other))
        {
            if (other.TryPush(direction))
            {
                Move(target);
                return true;
            }
            return false;
        }
        return false;
    }

    void Move(Vector3 target)
    {
        _parent.transform.position = target;
    }
}