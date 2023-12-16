using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    public static Action OnArrival;
    public static Action OnDepart;
    Awaitable _move;
    [SerializeField] float _moveSpeed;

    void Start()
    {
        _move = Lerper.MoveTo(gameObject, Vector3.zero);
    }

    void Update()
    {
        Vector3 input = new Vector3(Mathf.Round(Input.GetAxisRaw("Horizontal")), Mathf.Round(Input.GetAxisRaw("Vertical")), 0f);
        if (input.x > 0f && input.y > 0f)
            input = new Vector3(input.x, 0f, 0f);

        Vector3 target = transform.position + input;
        if (input.magnitude > 0f && _move.IsCompleted)
        {
            if (Pushable.TryGetAt(target, out Pushable pushable) && pushable.TryPush(input))
            {
                _move = Lerper.MoveTo(gameObject, new Vector3(input.x, input.y, 0f));
                return;
            }
            if (!Physics2D.OverlapCircle(target, 0.25f, LayerMask.GetMask("Default")) && pushable == null)
                _move = Lerper.MoveTo(gameObject, new Vector3(input.x, input.y, 0f));
        }
    }
}
