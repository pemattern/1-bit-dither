using System.Collections.Generic;
using UnityEngine;
using System;
public class MovementScheduler : MonoBehaviour
{
    public static Action OnBeginMove;
    public static Action OnUpdateMove;
    public static Action OnCompletedMove;
    public static bool Locked;
    private static MovementScheduler _instance;
    private Dictionary<Transform, Vector3> _unitsToMove;
    private int _completedMoves;

    void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        _unitsToMove = new Dictionary<Transform, Vector3>();
    }

    public static void Add(Transform transform, Vector3 direction)
    {
        if (!_instance._unitsToMove.ContainsKey(transform))
            _instance._unitsToMove.Add(transform, direction);
    }

    public static void Launch()
    {
        Locked = true;
        OnBeginMove?.Invoke();
        foreach (KeyValuePair<Transform, Vector3> unit in _instance._unitsToMove)
        {
            _instance.StartCoroutine(Lerper.MoveTo(unit.Key, unit.Value, _instance.UpdateCompletion));
        }
    }

    private void UpdateCompletion()
    {
        _completedMoves++;
        if (_completedMoves == _unitsToMove.Count)
        {
            Reset();
        }
    }

    private void Reset()
    {
        _completedMoves = 0;
        Locked = false;
        _unitsToMove.Clear();
        OnCompletedMove?.Invoke();
    }
};