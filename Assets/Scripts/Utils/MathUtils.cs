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

    /**
     * Calculates the determinant of 3 points
     * **/
    static public float Determinant(Vector2 u, Vector2 v, Vector2 w)
    {
        Vector2 vec1 = u - w;
        Vector2 vec2 = v - w;

        return Determinant(vec1, vec2);
    }

    /**
     * Calculates the determinant of 2 vectors
     * **/
    static public float Determinant(Vector2 u, Vector2 v)
    {
        return u.x * v.y - u.y * v.x;
    }
}
