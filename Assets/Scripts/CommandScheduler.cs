using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class CommandScheduler : MonoBehaviour
{
    public static bool Locked { get; private set; }
    public static Action OnBeginCommand;
    public static Action OnUpdateCommand;
    public static Action OnCompletedCommand;
    private static CommandScheduler _instance;
    private Stack<List<ICommand>> _commandHistory;
    private List<ICommand> _scheduledCommands;

    void Awake() { _instance = this; }

    void Start() 
    {
         _commandHistory = new Stack<List<ICommand>>(); 
         _scheduledCommands = new List<ICommand>();
         Locked = false; 
    }

    public static void Add(ICommand command)
    {
        _instance._scheduledCommands.Add(command);
    }

    public static async Task Execute()
    {
        _instance._commandHistory.Push(new List<ICommand>(_instance._scheduledCommands));
        Locked = true;
        OnBeginCommand?.Invoke();
        List<Task> tasks = new List<Task>();
        foreach (ICommand command in _instance._scheduledCommands)
        {
            tasks.Add(command.Do());
        }
        while (tasks.Where(x => !x.IsCompleted).Any())
        {
            OnUpdateCommand?.Invoke();
            await Awaitable.NextFrameAsync();
        }
        _instance._scheduledCommands.Clear();
        OnCompletedCommand?.Invoke();
        Locked = false;
    }

    public static async Task Undo()
    {
        if (!_instance._commandHistory.TryPop(out List<ICommand> commands)) return;
        Locked = true;
        OnBeginCommand?.Invoke();
        List<Task> tasks = new List<Task>();
        foreach(ICommand command in commands)
        {
            tasks.Add(command.Undo());
        }
        while (tasks.Where(x => !x.IsCompleted).Any())
        {
            OnUpdateCommand?.Invoke();
            await Awaitable.NextFrameAsync();
        }
        OnCompletedCommand?.Invoke();
        Locked = false;
    }
}
