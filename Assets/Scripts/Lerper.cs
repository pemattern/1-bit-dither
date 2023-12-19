using System;
using System.Collections;
using UnityEngine;
public class Lerper : MonoBehaviour
{
    private static Lerper _instance;
    [SerializeField] private float _lerpSpeed = 10;

    void Awake() { _instance = this; }

    public static IEnumerator MoveTo(Transform transform, Vector3 direction, Action completionCallback)
    {
        Vector3 target = transform.position + direction;
        while(Vector3.Distance(transform.position, target) > 0.05f)
        {
            transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * _instance._lerpSpeed);
            yield return null;
        }
        transform.position = new Vector3(Mathf.RoundToInt(target.x), Mathf.RoundToInt(target.y), 0f);
        completionCallback();
    }
}