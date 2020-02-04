using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private (float, float) zoombounds = (0.4f, 20f);
    private (float, float) earthYShiftUnadjusted = (-6.3f, -5.3f);
    private (float, float) earthScaleFactorUnadjusted = (1.2f, 0.8f);
    private float defaultOrthographicSize = 2.5f;
    private float defaultBackgroundScale = 25f;
    private float defaultEarthScale = 20f;
    private GameObject backgroundSprite;
    private GameObject earthImage;
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
        earthImage = GameObject.Find("EarthImage");
        Input.multiTouchEnabled = true;
        Camera.main.orthographicSize = defaultOrthographicSize;
        Camera.main.transform.position = new Vector3(0, 0, -10);

        float earthImageScale = defaultEarthScale / defaultOrthographicSize * Camera.main.orthographicSize;
        float a = (Camera.main.orthographicSize - zoombounds.Item1) / (zoombounds.Item2 - zoombounds.Item1);
        float b = a * (earthScaleFactorUnadjusted.Item2 - earthScaleFactorUnadjusted.Item1) + earthScaleFactorUnadjusted.Item1;
        earthImageScale *= b;
        earthImage.transform.localScale = new Vector3(earthImageScale, earthImageScale, 0);

        float c = a * (earthYShiftUnadjusted.Item2 - earthYShiftUnadjusted.Item1) + earthYShiftUnadjusted.Item1;
        earthImage.transform.localPosition = new Vector3(0, c / defaultOrthographicSize * Camera.main.orthographicSize, 11);
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
            touchStart.z = 0;
        }
        if (Input.GetMouseButton(0))
        {
            if (Util.OnCanvas(Camera.main.WorldToScreenPoint(touchStart)))
            {
                Vector3 touch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                touch.z = 0;
                Camera.main.transform.position += touchStart - touch;
            }
        }
        Zoom(Input.mouseScrollDelta.y*0.5f);
    }

    void Zoom(float increment)
    {
        Camera.main.orthographic = true;
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - increment, zoombounds.Item1, zoombounds.Item2);
        float backgroundScale = defaultBackgroundScale / defaultOrthographicSize * Camera.main.orthographicSize;
        float earthImageScale = defaultEarthScale / defaultOrthographicSize * Camera.main.orthographicSize;
        float a = (Camera.main.orthographicSize - zoombounds.Item1) / (zoombounds.Item2 - zoombounds.Item1);
        float b = a * (earthScaleFactorUnadjusted.Item2 - earthScaleFactorUnadjusted.Item1) + earthScaleFactorUnadjusted.Item1;
        earthImageScale *= b;
        backgroundSprite.transform.localScale = new Vector3(backgroundScale, backgroundScale, 0);
        earthImage.transform.localScale = new Vector3(earthImageScale, earthImageScale, 0);
        // old earthimage position
        float c = a * (earthYShiftUnadjusted.Item2 - earthYShiftUnadjusted.Item1) + earthYShiftUnadjusted.Item1;
        earthImage.transform.localPosition = new Vector3(0, c / defaultOrthographicSize * Camera.main.orthographicSize, 11);
    }
}
