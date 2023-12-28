using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class Activatable : MonoBehaviour
{
    [SerializeField] private GameObject[] _gameObjects;
    [SerializeField] private Logic _activationLogic;
    private List<IActivator> _activators;
    void Start()
    {
        _activators = new List<IActivator>();
        foreach (GameObject gameObject in _gameObjects)
        {
            if (!gameObject.TryGetComponent(out IActivator activator))
                throw new System.Exception("No Activator attached to object");

            _activators.Add(activator);
        }
    }

    public bool IsActivated()
    {
        switch (_activationLogic)
        {
            case Logic.Or: return _activators.Where(x => x.IsActivated()).Any();
            case Logic.And: return !_activators.Where(x => !x.IsActivated()).Any();
            default: return false;
        }
        
    }

    enum Logic
    {
        Or,
        And
    }
}