﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private float zoomMax = 20f;
    private float zoomMin = 0.4f;
    private float defaultOrthographicSize = 2.5f;
    private float defaultBackgroundScale = 20f;
    private GameObject backgroundSprite;
    private Vector3 touchStart;
    private int lastTouchCnt = 0;

    // Update is called once per frame
    void Update()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            UpdateMobile();
        }
        else
        {
            UpdateDesktop();
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    private float cameraHeight
    {
        get {return Camera.main.orthographicSize * 2f;}
    }

    private float cameraHalfHeight
    {
        get { return Camera.main.orthographicSize; }
    }

    private float cameraWidth
    {
        get {return cameraHeight * Camera.main.aspect; }
    }

    private float cameraHalfWidth
    {
        get { return Camera.main.orthographicSize * Camera.main.aspect; }
    }

    private void Awake()
    {
        backgroundSprite = GameObject.Find("GameBackground");
        Input.multiTouchEnabled = true;
        Camera.main.orthographicSize = defaultOrthographicSize;
        Camera.main.transform.position = new Vector3(0, 0, -10);
    }

    private void UpdateMobile()
    {
        if (Input.touchCount == 1)
        {
            if (Input.GetMouseButtonDown(0) || lastTouchCnt == 2)
            {
                touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
            if (Input.GetMouseButton(0))
            {
                if (Util.OnCanvas(Camera.main.WorldToScreenPoint(touchStart)))
                {
                    //so that you can't go further which would cause floating point issues;
                    Vector3 touch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Vector3 newpos = Camera.main.transform.position + touchStart - touch;
                    float newxpos = Mathf.Clamp(newpos.x, -Util.MAXFLOAT + cameraHalfWidth, Util.MAXFLOAT - cameraHalfWidth);
                    float newypos = Mathf.Clamp(newpos.y, -Util.MAXFLOAT + cameraHalfHeight, Util.MAXFLOAT - cameraHalfHeight);
                    newpos.x = newxpos; newpos.y = newypos; newpos.z = -10;
                    Camera.main.transform.position = newpos;
                }
            }
            lastTouchCnt = 1;
        }
        else if (Input.touchCount == 2)
        {
            //zooming camera
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);
            ////if both touches are not in the scrollview;

            if (Util.OnCanvas(touchZero.position) && Util.OnCanvas(touchOne.position))
            {
                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                float prevDis = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float currDis = (touchZero.position - touchOne.position).magnitude;

                float diff = currDis - prevDis;
                Zoom(diff * 0.01f);
            }
            lastTouchCnt = 2;
        }
    }

    //for desktops
    private void UpdateDesktop()
    {
        if (Input.GetMouseButtonDown(0))
        {
            touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        if (Input.GetMouseButton(0))
        {
            if (Util.OnCanvas(Camera.main.WorldToScreenPoint(touchStart)))
            {
                Vector3 touch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Camera.main.transform.position += touchStart - touch;
            }
        }
        Zoom(Input.mouseScrollDelta.y*0.5f);
    }

    void Zoom(float increment)
    {
        Camera.main.orthographic = true;
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - increment, zoomMin, zoomMax);
        float backgroundScale = defaultBackgroundScale / defaultOrthographicSize * Camera.main.orthographicSize;
        backgroundSprite.GetComponent<Transform>().localScale = new Vector3(backgroundScale, backgroundScale, 0);
    }
}