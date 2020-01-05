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
    public static bool sliderSelected = false;
    public static bool objectDragged = false;
    public static float EPSILON = 0.000001f;

    public static float MAXFLOAT = 9999.99f;

    public static Color shadedColor = new Color(189f / 255f, 204f / 255f, 255f / 255f, 1); //light blue
    public static Color unshadedColor = new Color(1, 1, 1, 1); //white

    public static bool OnCanvas (Vector3 screenPos)
    {
        RectTransform scrollViewRect = scrollView.transform as RectTransform;
        return !RectTransformUtility.RectangleContainsScreenPoint(scrollViewRect, screenPos) && !sliderSelected && !objectDragged;
    }

    public static float GetAngleFromVectorFloat (Vector2 v)
    { 
        return Mathf.Rad2Deg*Mathf.Atan2(v.y, v.x);
    }

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

    void Update()
    {
        
    }

    private void Awake()
    {
        scrollView = GameObject.Find("ObjectsScrollView");
    }
}