﻿using System.Collections;
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

    public static float MAXFLOAT = 9999.99f;

    public static bool OnCanvas (Vector3 screenPos)
    {
        RectTransform scrollViewRect = scrollView.transform as RectTransform;
        return !RectTransformUtility.RectangleContainsScreenPoint(scrollViewRect, screenPos) && !sliderSelected && !objectDragged;
    }

    public static float GetAngleFromVectorFloat (Vector2 v)
    { 
        return Mathf.Rad2Deg*Mathf.Atan2(v.y, v.x);
    }

    void Update()
    {
        
    }

    private void Awake()
    {
        scrollView = GameObject.Find("ObjectsScrollView");
    }
}