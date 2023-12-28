using UnityEngine;

public class PressurePlate : MonoBehaviour, IActivator
{
    public bool IsActivated()
    {
         return Physics2D.OverlapCircle(transform.position, Consts.OverlapCircleRadius, LayerMask.GetMask("Pushable"));
    }
}
