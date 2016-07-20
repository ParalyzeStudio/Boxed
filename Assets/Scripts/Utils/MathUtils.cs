using UnityEngine;

public class MathUtils
{
    public static float DotProduct(Vector3 u, Vector3 v)
    {
        return u.x * v.x + u.y * v.y + u.z * v.z;
    }

    //public static float DotProduct(Vector2 u, Vector2 v)
    //{
    //    return u.x * v.x + u.y * v.y;
    //}
}
