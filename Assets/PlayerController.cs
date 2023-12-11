using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static Action<Vector2> onMove;
    void Update()
    {
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f);

        if (input.magnitude > 0)
        {
            transform.position += input * Time.deltaTime;
            onMove.Invoke(transform.position);
        } 
    }
}
