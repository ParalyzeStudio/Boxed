using UnityEngine;

public class ColorUtils
{
    public static Color GetColorFromRGBAVector4(Vector4 rgbaVec4)
    {
        return new Color(rgbaVec4.x / 255.0f, rgbaVec4.y / 255.0f, rgbaVec4.z / 255.0f, rgbaVec4.w / 255.0f);
    }

    public static Color GetRGBAVector4FromColor(Color color)
    {
        return new Color(color.r * 255.0f, color.g * 255.0f, color.b * 255.0f, color.a * 255.0f);
    }

    public static Color DarkenColor(Color inColor, float t)
    {
        Color outColor = Color.Lerp(inColor, Color.black, t);
        outColor.a = inColor.a; //let opacity unchanged
        return outColor;
    }

    public static Color LightenColor(Color inColor, float t)
    {
        Color outColor = Color.Lerp(inColor, Color.white, t);
        outColor.a = inColor.a; //let opacity unchanged
        return outColor;
    }

    public static Color FadeColor(Color color, float opacity)
    {
        return new Color(color.r, color.g, color.b, opacity);
    }

    public static Color IntensifyColorChannels(Color color, bool rChannel, bool gChannel, bool bChannel, float rt, float gt, float bt)
    {
        return new Color(rChannel ? Mathf.Lerp(color.r, 1, rt) : color.r,
                         gChannel ? Mathf.Lerp(color.g, 1, gt) : color.g,
                         bChannel ? Mathf.Lerp(color.b, 1, bt) : color.b,
                         color.a);
    }

    /**
     * Returns the color from the rainbow of colors (red-orange-yellow-yellow-green-cyan-blue-pink-purple-red)
     * **/
    public static Color GetRainbowColorAtPercentage(float percentage)
    {
        if (percentage < 0 || percentage > 100)
            throw new System.Exception("Percentage has to be a float number between 0 and 100");

        //List here colors that will serve as milestones (linear interpolation between two consecutive colors)
        Color[] milestones = new Color[6];
        milestones[0] = new Color(1, 0, 0, 1); //red
        milestones[1] = new Color(1, 1, 0, 1); //yellow
        milestones[2] = new Color(0, 1, 0, 1); //green
        milestones[3] = new Color(0, 1, 1, 1); //cyan
        milestones[4] = new Color(0, 0, 1, 1); //blue
        milestones[5] = new Color(1, 0, 1, 1); //purple

        //then find the correct tier where the percentage belongs
        int integerPercentage = (int)Mathf.RoundToInt(percentage * 100000);
        int tierLength = (int)Mathf.RoundToInt(10000000 / 6.0f); //6 tiers, 1 666 667 samples in each tier
        int tierIndex = integerPercentage / tierLength;
        float tierLocalPercentage = (float)integerPercentage % tierLength;
        tierLocalPercentage /= (float)tierLength;

        Color startColor = milestones[tierIndex];
        Color endColor = milestones[(tierIndex == 5) ? 0 : tierIndex + 1];

        return Color.Lerp(startColor, endColor, tierLocalPercentage);
    }

    /**
     * Gets a random color that is near the passed original color
     * The amplitude determines the maxismum values each channel R,G,B can differ from the original value
     * **/
    public static Color GetRandomNearColor(Color originalColor, float amplitude)
    {
        float randomValue = UnityEngine.Random.value;
        randomValue *= (2 * amplitude);
        randomValue -= amplitude; //[-amplitude; +amplitude]

        float nearColorRed = originalColor.r + randomValue;
        float nearColorGreen = originalColor.g + randomValue;
        float nearColorBlue = originalColor.b + randomValue;

        nearColorRed = Mathf.Clamp(nearColorRed, 0, 1);
        nearColorGreen = Mathf.Clamp(nearColorGreen, 0, 1);
        nearColorBlue = Mathf.Clamp(nearColorBlue, 0, 1);

        return new Color(nearColorRed, nearColorGreen, nearColorBlue, originalColor.a);
    }

    /**
     * Return a random color
     * **/
    public static Color GetRandomColor(float opacity = 1.0f)
    {
        float r = Random.value;
        float g = Random.value;
        float b = Random.value;

        return new Color(r, g, b, opacity);
    }

    public static Color GetRGBAColorFromHSL(Vector3 hsl, float a)
    {
        float hue = hsl.x;
        float saturation = hsl.y;
        float lightness = hsl.z;
        float C = 1 - Mathf.Abs(2 * lightness - 1) * saturation;

        hue = hue % 360.0f;
        if (hue < 0)
            hue = 360 - Mathf.Abs(hue);
        hue /= 60.0f;
        float X = C * (1 - Mathf.Abs(hue % 2.0f - 1));

        Color outColor;
        if (0 <= hue && hue < 1)
            outColor = new Color(C, X, 0, a);
        else if (1 <= hue && hue < 2)
            outColor = new Color(X, C, 0, a);
        else if (2 <= hue && hue < 3)
            outColor = new Color(0, C, X, a);
        else if (3 <= hue && hue < 4)
            outColor = new Color(0, X, C, a);
        else if (4 <= hue && hue < 5)
            outColor = new Color(X, 0, C, a);
        else if (5 <= hue && hue < 6)
            outColor = new Color(C, 0, X, a);
        else
            outColor = new Color(0, 0, 0, a);

        float m = lightness - 0.5f * C;
        outColor += new Color(m, m, m, 0);

        return outColor;
    }
    

    public static Vector3 GetHSLFromRGBAColor(Color rgba)
    {
        float[] channels = new float[3];
        channels[0] = rgba.r;
        channels[1] = rgba.g;
        channels[2] = rgba.b;
        float min = Mathf.Min(channels);
        float max = Mathf.Max(channels);

        //Hue
        float hue = 0;
        if (min != max)
        {
            if (max == rgba.r)
            {
                hue = (60 * (rgba.g - rgba.b) / (max - min) + 360);
                hue %= 360;
            }
            else if (max == rgba.g)
            {
                hue = 60 * (rgba.b - rgba.r) / (max - min) + 120;
            }
            else //max == rgba.b
            {
                hue = 60 * (rgba.r - rgba.g) / (max - min) + 240;
            }
        }

        //Lightness
        float lightness = 0.5f * (max + min);

        //Saturation
        float saturation = 0;
        if (min != max)
            saturation = (max - min) / (1 - Mathf.Abs(2 * lightness - 1));

        return new Vector3(hue, saturation, lightness);
    }

    //public static void UnitTests()
    //{
    //    Color color1 = Color.black;
    //    Vector3 hsv1 = new Vector3(0,0,0);
    //    Vector3 hsl1 = new Vector3(0,0,0);

    //    if (GetHSVFromRGBAColor(color1) == hsv1 && GetHSLFromRGBAColor(color1) == hsl1)
    //        Debug.Log("Color Test1 SUCCESS");
    //    else
    //        Debug.Log("Color Test1 FAILURE");

    //    Color color2 = Color.white;
    //    Vector3 hsv2 = new Vector3(0, 0, 1);
    //    Vector3 hsl2 = new Vector3(0, 0, 1);

    //    if (GetHSVFromRGBAColor(color2) == hsv2 && GetHSLFromRGBAColor(color2) == hsl2)
    //        Debug.Log("Color Test2 SUCCESS");
    //    else
    //        Debug.Log("Color Test2 FAILURE");

    //    Color color3 = Color.blue;
    //    Vector3 hsv3 = new Vector3(240, 1, 1);
    //    Vector3 hsl3 = new Vector3(240, 1, 0.5f);

    //    if (GetHSVFromRGBAColor(color3) == hsv3 && GetHSLFromRGBAColor(color3) == hsl3)
    //        Debug.Log("Color Test3 SUCCESS");
    //    else
    //        Debug.Log("Color Test3 FAILURE");

    //    Color color4 = new Color(153 / 255.0f,10 / 255.0f, 68 / 255.0f);
    //    Vector3 hsv4 = new Vector3(336, 0.877f, 0.32f);
    //    Vector3 hsl4 = new Vector3(336, 0.935f, 0.6f);

    //    float distanceHSV = (hsv4 - GetHSVFromRGBAColor(color4)).magnitude;
    //    float distanceHSL = (hsl4 - GetHSLFromRGBAColor(color4)).magnitude;

    //    if (distanceHSV <= 1f && distanceHSL <= 1f)
    //        Debug.Log("Color Test4 SUCCESS");
    //    else
    //        Debug.Log("Color Test4 FAILURE");

    //    Color color5 = new Color(132 / 255.0f, 69 / 255.0f, 214 / 255.0f);
    //    Vector3 hsv5 = new Vector3(266, 0.678f, 0.839f);
    //    Vector3 hsl5 = new Vector3(266, 0.639f, 0.555f);

    //    distanceHSV = (hsv5 - GetHSVFromRGBAColor(color5)).magnitude;
    //    distanceHSL = (hsl5 - GetHSLFromRGBAColor(color5)).magnitude;

    //    if (distanceHSV <= 1f && distanceHSL <= 1f)
    //        Debug.Log("Color Test5 SUCCESS");
    //    else
    //        Debug.Log("Color Test5 FAILURE");
    //}
}

public struct HSVColor
{
    Vector3 m_hsv;

    public HSVColor(float hue, float saturation, float value)
    {
        m_hsv.x = hue;
        m_hsv.y = saturation;
        m_hsv.z = value;
    }

    public HSVColor(Color rgb)
    {
        float[] channels = new float[3];
        channels[0] = rgb.r;
        channels[1] = rgb.g;
        channels[2] = rgb.b;
        float min = Mathf.Min(channels);
        float max = Mathf.Max(channels);

        //Hue
        float hue = 0;
        if (min != max)
        {
            if (max == rgb.r)
            {
                hue = (60 * (rgb.g - rgb.b) / (max - min) + 360);
                hue %= 360;
            }
            else if (max == rgb.g)
            {
                hue = 60 * (rgb.b - rgb.r) / (max - min) + 120;
            }
            else //max == rgba.b
            {
                hue = 60 * (rgb.r - rgb.g) / (max - min) + 240;
            }
        }

        //Saturation
        float saturation = 0;
        if (min != max)
            saturation = 1 - min / max;

        //Value
        float value = max;

        m_hsv = new Vector3(hue, saturation, value);
    }

    public Color ToRGBA(float a = 1)
    {
        float hue = m_hsv.x;
        float saturation = m_hsv.y;
        float value = m_hsv.z;
        float C = saturation * value;

        hue = hue % 360.0f;
        if (hue < 0)
            hue = 360 - Mathf.Abs(hue);
        hue /= 60.0f;
        float X = C * (1 - Mathf.Abs(hue % 2.0f - 1));

        Color outColor;
        if (0 <= hue && hue < 1)
            outColor = new Color(C, X, 0, a);
        else if (1 <= hue && hue < 2)
            outColor = new Color(X, C, 0, a);
        else if (2 <= hue && hue < 3)
            outColor = new Color(0, C, X, a);
        else if (3 <= hue && hue < 4)
            outColor = new Color(0, X, C, a);
        else if (4 <= hue && hue < 5)
            outColor = new Color(X, 0, C, a);
        else if (5 <= hue && hue < 6)
            outColor = new Color(C, 0, X, a);
        else
            outColor = new Color(0, 0, 0, a);

        float m = value - C;
        outColor += new Color(m, m, m, 0);

        return outColor;
    }

    public void TranslateHue(float deltaHue)
    {
        m_hsv.x = (m_hsv.x + deltaHue) % 360;
    }

    public float GetHue()
    {
        return m_hsv.x;
    }

    public float GetSaturation()
    {
        return m_hsv.y;
    }

    public float GetValue()
    {
        return m_hsv.z;
    }

    public void SetHue(float hue)
    {
        m_hsv.x = hue;
    }

    public void SetSaturation(float saturation)
    {
        m_hsv.y = saturation;
    }

    public void SetValue(float value)
    {
        m_hsv.z = value;
    }
}