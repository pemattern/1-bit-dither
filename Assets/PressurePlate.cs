using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    void OnEnable()
    {
        CommandScheduler.OnCompletedCommand += CheckForPushables;
    }
    void CheckForPushables()
    {
        if (!Physics2D.OverlapCircle(transform.position, Consts.OverlapCircleRadius, LayerMask.GetMask("Pushable"))) return;

        Debug.Log("Pushed");
    }

    void OnDisable()
    {
        CommandScheduler.OnCompletedCommand -= CheckForPushables;
    }
}
