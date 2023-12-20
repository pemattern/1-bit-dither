using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using System.Linq;
public class MovementScheduler : MonoBehaviour
{
    public static event Action OnBeginMove;
    public static event Action OnUpdateMove;
    public static event Action OnCompletedMove;
    public static bool Locked;
    private static MovementScheduler _instance;
    private Dictionary<Transform, Vector3> _unitsToMove;
    private List<Task> _moves;

    void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        _unitsToMove = new Dictionary<Transform, Vector3>();
        _moves = new List<Task>();
    }

    public static void Add(Transform transform, Vector3 direction)
    {
        if (!_instance._unitsToMove.ContainsKey(transform))
            _instance._unitsToMove.Add(transform, direction);
    }

    public static async Task Launch()
    {
        Locked = true;
        OnBeginMove?.Invoke();
        foreach (KeyValuePair<Transform, Vector3> unit in _instance._unitsToMove)
        {
            _instance._moves.Add(Lerper.MoveTo(unit.Key, unit.Value));
        }
        while (_instance._moves.Where(x => !x.IsCompleted).Any())
        {
            OnUpdateMove?.Invoke();
            await Awaitable.NextFrameAsync();
        }
        _instance.Reset();
    }

    private void Reset()
    {
        Locked = false;
        _unitsToMove.Clear();
        _moves.Clear();
        LightProber.UpdateLightTexture();
        OnCompletedMove?.Invoke();
    }
};