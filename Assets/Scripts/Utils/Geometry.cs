﻿using UnityEngine;

public class Geometry
{
    /**
    * Determine the intersection between a line and plane knowing this intersection is unique:
    * -line and plane are not parallel (0 solution)
    * -line is not contained inside the plane (infinite number of solutions)
    **/
    //public static void LinePlaneIntersection(Vector3 linePoint, Vector3 lineDirection, Vector3 planePoint, Vector3 planeNormal, out bool intersects, out Vector3 intersection)
    //{
    //    if (MathUtils.DotProduct(lineDirection, planeNormal) == 0)
    //    {
    //        intersects = false;
    //        intersection = Vector3.zero;
    //        return;
    //    }

    //    //Change variable names for greater clarity and shorter names
    //    float xA = planePoint.x;
    //    float yA = planePoint.y;
    //    float zA = planePoint.z;

    //    float a = planeNormal.x;
    //    float b = planeNormal.y;
    //    float c = planeNormal.z;
    //    float d = -a * xA - b * yA - c * zA;

    //    float x0 = linePoint.x;
    //    float y0 = linePoint.y;
    //    float z0 = linePoint.z;

    //    float alpha = lineDirection.x;
    //    float beta = lineDirection.y;
    //    float gamma = lineDirection.z;

    //    //theres one unique solution given by the parameter t0 which we reinject then in the parametric equation of the line
    //    float t0 = -(a * x0 + b * y0 + c * z0) / (a * alpha + b * beta + c * gamma);

    //    intersects = true;
    //    intersection = new Vector3(x0 + alpha * t0,
    //                               y0 + beta * t0,
    //                               z0 + gamma * t0);
    //}


    public struct Edge
    {
        public Vector3 m_pointA;
        public Vector3 m_pointB;

        public Edge(Vector3 pointA, Vector3 pointB)
        {
            m_pointA = pointA;
            m_pointB = pointB;
        }

        public Vector3 GetMiddle()
        {
            return 0.5f * (m_pointA + m_pointB);
        }

        public void ApplyRotation(Quaternion rotation)
        {
            m_pointA = rotation * m_pointA;
            m_pointB = rotation * m_pointB;
        }

        public void Translate(Vector3 dPos)
        {
            m_pointA += dPos;
            m_pointB += dPos;
        }
    }

    public static Vector2 RemoveYComponent(Vector3 point)
    {
        return new Vector2(point.x, point.z);
    }


    public struct Triangle
    {
        public Vector3[] m_points;

        public Triangle(Vector3 A, Vector3 B, Vector3 C)
        {
            m_points = new Vector3[3];
            m_points[0] = A;
            m_points[1] = B;
            m_points[2] = C;
        }

        public bool ContainsPoint(Vector2 point, bool bStrictlyInside = false)
        {
            float det1 = MathUtils.Determinant(m_points[0], m_points[1], point);
            float det2 = MathUtils.Determinant(m_points[1], m_points[2], point);
            float det3 = MathUtils.Determinant(m_points[2], m_points[0], point);

            if (bStrictlyInside)
                return (det1 < 0 && det2 < 0 && det3 < 0) || (det1 > 0 && det2 > 0 && det3 > 0);
            else
                return (det1 <= 0 && det2 <= 0 && det3 <= 0) || (det1 >= 0 && det2 >= 0 && det3 >= 0);
        }
    }
}
