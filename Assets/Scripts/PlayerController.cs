using UnityEngine;

public class PlayerController : MonoBehaviour
{
    void Update()
    {
        Vector3 input = new Vector3(Mathf.Round(Input.GetAxisRaw("Horizontal")), Mathf.Round(Input.GetAxisRaw("Vertical")), 0f);
        if (input.x > 0f && input.y > 0f)
            input = new Vector3(input.x, 0f, 0f);

        Vector3 target = transform.position + input;
        if (input.magnitude > 0f && !MovementScheduler.Locked)
        {
            if (Physics2D.OverlapCircle(target, Consts.OverlapCircleRadius, LayerMask.GetMask("Default")) || 
                (Pushable.TryGetAt(target, out Pushable pushable) && !pushable.TryPush(input))) return;

            MovementScheduler.Add(transform, input);
            MovementScheduler.Launch();
        }
    }
}
