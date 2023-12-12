using System;
using System.Collections;
using System.Collections.Generic;
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
        //transform.position += input * Time.deltaTime * _moveSpeed;

        if (Vector3.Distance(transform.position, _playerTarget.position) <= 0.05f)
        {
            if (input.x != 0)
            {
                _playerTarget.position = new Vector3(
                    Mathf.Round(transform.position.x) + input.x,
                    Mathf.Round(transform.position.y),
                    0f
                );
            }
            else if (input.y != 0)
            {
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
