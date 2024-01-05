using UnityEngine;

public class Direction
{
    public enum FourWay
    {
        Up,
        Right,
        Down,
        Left
    }

    public static FourWay Rotate(FourWay currentDirection, bool clockwise = true)
    {
        return currentDirection switch
        {
            FourWay.Up => clockwise ? FourWay.Right : FourWay.Left,
            FourWay.Right => clockwise ? FourWay.Down : FourWay.Up,
            FourWay.Down => clockwise ? FourWay.Left : FourWay.Right,
            FourWay.Left => clockwise ? FourWay.Up : FourWay.Down,
            _ => FourWay.Up
        };
    }

    public static Vector3 Rotate(Vector3 currentDirection, bool clockwise = true)
    {
        int sign = clockwise ? -1 : 1;
        return Quaternion.Euler(0f, 0f, sign * 90) * currentDirection;        
    }

    public static Vector3 AsVector(FourWay direction)
    {
        return direction switch
        {
            FourWay.Up => Vector3.up,
            FourWay.Right => Vector3.right,
            FourWay.Down => Vector3.down,
            FourWay.Left => Vector3.left,
            _ => Vector3.zero,
        };
    }
}