using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    private static PlayerController _instance;
    private Awaitable _inputDelay;
    private Vector3 _inputBuffer;
    public static Direction.FourWay Facing { get; private set; }
    public static Vector3 Position => _instance.transform.position;
    void Awake()
    {
        _instance = this;
    }
    async void Update()
    {
        if (Input.GetKey(KeyCode.Backspace) && !CommandScheduler.Locked)
        {
            _inputDelay = Awaitable.WaitForSecondsAsync(Consts.InputDelay);
            await CommandScheduler.Undo();
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
            _inputDelay = Awaitable.WaitForSecondsAsync(Consts.InputDelay);
            Vector3 target = transform.position + input;

            bool immoveable = Physics2D.OverlapCircle(target, Consts.OverlapCircleRadius, LayerMask.GetMask("Default", "Lightpassthrough")) || 
                (Pushable.TryGetAt(target, out Pushable pushable) && !pushable.TryPush(input));

            Facing = Direction.AsFourWay(input);
            CommandScheduler.Add(new Movement(transform, input, immoveable));
            await CommandScheduler.Execute();
        }
    }
}
