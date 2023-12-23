using System.Threading.Tasks;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Awaitable _inputDelay;
    private Vector3 _inputBuffer;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace) && !CommandScheduler.Locked)
        {
            CommandScheduler.Undo();
            _inputDelay = Awaitable.WaitForSecondsAsync(Consts.InputDelay);
        }

        if (CommandScheduler.Locked && _inputDelay != null && _inputDelay.IsCompleted)
        {
            Vector3 temp = new Vector3(Mathf.Round(Input.GetAxisRaw("Horizontal")), Mathf.Round(Input.GetAxisRaw("Vertical")), 0f);
            if (temp.x != 0f && temp.y != 0f)
                _inputBuffer = new Vector3(temp.x, 0f, 0f);
        }

        Vector3 input;
        if (_inputBuffer.magnitude > 0f)
        {
            input = _inputBuffer;
            _inputBuffer = Vector3.zero;
        }
        else
        {
            input = new Vector3(Mathf.Round(Input.GetAxisRaw("Horizontal")), Mathf.Round(Input.GetAxisRaw("Vertical")), 0f);
            if (input.x != 0f && input.y != 0f)
                input = new Vector3(input.x, 0f, 0f);
        }

        if (input.magnitude > 0f && !CommandScheduler.Locked)
        {
            Vector3 target = transform.position + input;
            if (Physics2D.OverlapCircle(target, Consts.OverlapCircleRadius, LayerMask.GetMask("Default")) || 
                (Pushable.TryGetAt(target, out Pushable pushable) && !pushable.TryPush(input))) return;

            CommandScheduler.Add(new Move(transform, input));
            CommandScheduler.Execute();
            _inputDelay = Awaitable.WaitForSecondsAsync(Consts.InputDelay);
        }
    }
}
