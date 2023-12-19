using UnityEngine;

[RequireComponent(typeof(PixelOutline))]
public class InteractableLight : MonoBehaviour
{
    private PixelOutline _pixelOutline;
    void Start()
    {
        _pixelOutline = GetComponent<PixelOutline>();
    }
    void Check()
    {

    }
}
