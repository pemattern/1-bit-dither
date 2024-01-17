using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class Activatable : MonoBehaviour
{
    [SerializeField] private GameObject[] _gameObjects;
    [SerializeField] private Logic _activationLogic;
    [SerializeField] private bool _invertActivation;
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
        bool result;
        switch (_activationLogic)
        {
            case Logic.Or: result = _activators.Where(x => x.IsActivated()).Any(); break;
            case Logic.And: result = !_activators.Where(x => !x.IsActivated()).Any(); break;
            default: return false;
        }
        if (_invertActivation) return !result;
        return result;
    }

    enum Logic
    {
        Or,
        And
    }
}