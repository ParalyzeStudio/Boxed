using System;
using UnityEngine;

[System.Serializable]
public struct SerializableColor
{
    public float r;
    public float g;
    public float b;
    public float a;

    public SerializableColor(float rR, float rG, float rB, float rA)
    {
        r = rR;
        g = rG;
        b = rB;
        a = rA;
    }

    public override string ToString()
    {
        return String.Format("[{0}, {1}, {2}, {3}]", r, g, b, a);
    }

    public static SerializableColor operator -(SerializableColor a) { return new SerializableColor(-a.r, -a.g, -a.b, -a.a); }
    public static SerializableColor operator -(SerializableColor a, SerializableColor b) { return new SerializableColor(a.r - b.r, a.g - b.g, a.b - b.b, a.a - b.a); }
    public static bool operator ==(SerializableColor lhs, SerializableColor rhs) { return lhs.Equals(rhs); }
    public static bool operator !=(SerializableColor lhs, SerializableColor rhs) { return !lhs.Equals(rhs); }

    public override bool Equals(object obj)
    {
        if (!(obj is SerializableColor))
            return false;

        return Equals((SerializableColor) obj);
    }

    public bool Equals(SerializableColor other)
    {
        return r == other.r && g == other.g && b == other.b && a == other.a;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public static implicit operator Color(SerializableColor rValue)
    {
        return new Color(rValue.r, rValue.g, rValue.b, rValue.a);
    }

    public static implicit operator SerializableColor(Color rValue)
    {
        return new SerializableColor(rValue.r, rValue.g, rValue.b, rValue.a);
    }
}