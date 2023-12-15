using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float _moveSpeed;
    Transform _playerTarget;

    void Start()
    {
        _playerTarget = new GameObject("Player Target").transform;
    }
    void Update()
    {
        Vector3 input = new Vector3(Mathf.Round(Input.GetAxisRaw("Horizontal")), Mathf.Round(Input.GetAxisRaw("Vertical")), 0f);

        Vector3 target = transform.position + input;
        Pushable other = null;

            Collider2D[] colliders = Physics2D.OverlapCircleAll(target, 0.25f);//, LayerMask.NameToLayer("Pushable"));
            if(colliders.Length > 0)
            {
                if (colliders[0].TryGetComponent(out other))
                {
                    
                }
            }

        if (Vector3.Distance(transform.position, _playerTarget.position) <= 0.05f &&
            !Physics2D.OverlapCircle(target, 0.25f, LayerMask.NameToLayer("Default")))
        {
            transform.position = new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));
            if (input.x != 0)
            {
                other?.TryPush(input);
                _playerTarget.position = new Vector3(
                    Mathf.Round(transform.position.x) + input.x,
                    Mathf.Round(transform.position.y),
                    0f
                );
            }
            else if (input.y != 0)
            {
                other?.TryPush(input);
                _playerTarget.position = new Vector3(
                    Mathf.Round(transform.position.x),
                    Mathf.Round(transform.position.y) + input.y,
                    0f
                );
            }
        }

        transform.position = Vector3.Lerp(transform.position, _playerTarget.position, Time.deltaTime * _moveSpeed);
    }
}
