using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util: MonoBehaviour
{
    private static GameObject scrollView;
    public static bool sliderSelected = false;

    public static bool OnCanvas (Vector3 screenPos)
    {
        RectTransform scrollViewRect = scrollView.transform as RectTransform;
        return !RectTransformUtility.RectangleContainsScreenPoint(scrollViewRect, screenPos) && !sliderSelected;
    }

    void Update()
    {
        
    }

    void Start()
    {
        scrollView = GameObject.Find("ObjectsScrollView");
    }
}