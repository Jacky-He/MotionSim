using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GraphOptions
{
    positionX, positionY, velocityX, velocityY, accelerationX, accelerationY
}

public class Util: MonoBehaviour
{
    private static GameObject scrollView;
    private static GameObject propertiesControlArea;

    public static bool sliderSelected = false;
    public static bool objectDragged = false;
    public static float EPSILON = 0.00001f;
    public static float EPSILON_SMALL = 0.001f;

    public static float FixedRectWidthMultiplier = 1f;
    public static float FixedRectHeightMultiplier = 0.636595f;
    public static float MoveableRectWidthMultiplier = 1f;
    public static float MoveableRectHeightMultiplier = 0.62987f;
    public static float ForceHeightMultiplier = 1f;
    public static float SpringHeightMultiplier = 1f;
    public static float SpringWidthMultiplier = 0.2872265f;
    public static float VelocityHeightMultiplier = 1f;
    public static float CircleDiameterMultiplier = 1f;

    public static float WorldToScreenMultiplier = 180f;

    public static float MAXFLOAT = 9999.99f;

    //unit conversions
    public static float MetresToScreenUnits = 1f / 1f; //screenunits/metre
    public static float ScreenUnitsToMetres = 1f / 1f; //metres/screenunit;
    public static float GravitationalAcceleration = -9.8f; //-9.8 m/s^2

    //colors for displaying whether an objects is selected or not
    public static Color shadedColor = new Color(189f / 255f, 204f / 255f, 255f / 255f, 1); //light blue
    public static Color unshadedColor = new Color(1, 1, 1, 1); //white

    public static Color[] graphColors = new Color[6] { new Color(138f / 255f, 255f / 255f, 66f / 255), new Color(11f / 255f, 200f / 255f, 0f / 255f), new Color(123 / 255f, 222 / 255f, 243 / 255f), new Color(81 / 255f, 135 / 255f, 250 / 255f), new Color(255f / 255f, 205f / 255f, 119f / 255f), new Color(255f / 255f, 143f / 255f, 0f / 255f) };

    public static Color RightBackOffColor = new Color(255f / 255f, 142f / 255f, 76f / 255f, 0.5f);
    public static Color RightBackOnColor = new Color(255f / 255f, 142f / 255f, 76f / 255f, 1);

    public static Color LeftBackOffColor = new Color(255f / 255f, 142f / 255f, 76f / 255f, 0.5f);
    public static Color LeftBackOnColor = new Color(255f / 255f, 142f / 255f, 76f / 255f, 1);

    public static Color GraphTabOnColor = new Color(135f / 255f, 157f / 255f, 190f / 255f, 107f / 255f);
    public static Color GraphTabOffColor = new Color(120f / 255f, 120f / 255f, 120f / 255f, 80f / 255f);

    public static Color PropertiesTabOnColor = new Color(135f / 255f, 157f / 255f, 190f / 255f, 107f / 255f);
    public static Color PropertiesTabOffColor = new Color(120f / 255f, 120f / 255f, 120f / 255f, 80f / 255f);

    public static Font Caladea_Regular;
    public static Font Caladea_Bold;
    public static Font Caladea_Italic;
    public static Font Caladea_BoldItalic;

    //checks if the point is on empty spaces
    public static bool OnCanvas (Vector3 screenPos)
    {
        RectTransform scrollViewRect = scrollView.transform as RectTransform;
        RectTransform propConRect = propertiesControlArea.transform as RectTransform;
        return !RectTransformUtility.RectangleContainsScreenPoint(scrollViewRect, screenPos) && !sliderSelected && !objectDragged && !RectTransformUtility.RectangleContainsScreenPoint(propConRect, screenPos);
    }

    //get the angle based on the vector but does not account for the case where v.x = 0
    public static float GetAngleFromVectorFloat (Vector2 v)
    { 
        return Mathf.Rad2Deg*Mathf.Atan2(v.y, v.x);
    }

    //checks ifs the position of an object is out of bounds
    public static bool OutOfBound(Vector3 position)
    {
        return Mathf.Abs(position.x) > Util.MAXFLOAT || Mathf.Abs(position.y) > Util.MAXFLOAT;
    }

    //rotates a vector around its origin (0, 0) point according to the angledelta. + is counterclockwise and - is clockwise
    public static Vector3 RotateAroundOrigin (Vector2 vector, float angledelta)
    {
        float currangle = Mathf.Abs(vector.x) < Util.EPSILON ? (vector.y > 0f ? 90f : -90f) : GetAngleFromVectorFloat(vector);
        currangle += angledelta;
        currangle = (currangle + 720f) % 360f;
        Vector3 res = new Vector3();
        float magnitude = vector.magnitude;
        res.z = 0;
        res.x = magnitude * Mathf.Cos(Mathf.Deg2Rad * currangle);
        res.y = magnitude * Mathf.Sin(Mathf.Deg2Rad * currangle);
        return res;
    }

    public static float AngleBetweenTwoVectors (Vector2 vec1, Vector2 vec2)
    {
        return Vector2.SignedAngle(vec1, vec2);
    }

    void Update()
    {
        
    }

    private void Awake()
    {
        scrollView = GameObject.Find("ObjectsScrollView");
        propertiesControlArea = GameObject.Find("PropertiesControlArea");
        Caladea_Regular = (Font)Resources.Load("Fonts/caladea/Caladea-Regular");
        Caladea_Bold = (Font)Resources.Load("Fonts/caladea/Caladea-Bold");
        Caladea_Italic = (Font)Resources.Load("Fonts/caladea/Caladea-Italic");
        Caladea_BoldItalic = (Font)Resources.Load("Fonts/caladea/Caladea-BoldItalic");
    }
}