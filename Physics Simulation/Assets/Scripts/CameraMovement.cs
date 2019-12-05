using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Vector3 touchStart;
    private float zoomMax = 1000;
    private float zoomMin = 40;

    // Start is called before the first frame update
    void Start()
    {
        Input.multiTouchEnabled = true;
        Camera.main.orthographicSize = 257f;
        Camera.main.transform.position = new Vector3(456, 256.5f, -10);
    }

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

    private void UpdateMobile()
    {
        if (Input.touchCount == 1)
        {
            //true only the first time the mouse is touched down
            if (Input.GetMouseButtonDown(0))
            {
                touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
            //for draggin camera around (panning) Later also needs to check if they are on any objects
            if (Util.OnCanvas(Camera.main.WorldToScreenPoint(touchStart)) && Input.GetMouseButton(0))
            {
                Vector3 direction = touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Camera.main.transform.position += direction;
            }
        }
        else if (Input.touchCount == 2)
        {
            //zooming camera
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            //if both touches are not in the scrollview;
            if (Util.OnCanvas (Camera.main.WorldToScreenPoint(touchZero.position)) && Util.OnCanvas(Camera.main.WorldToScreenPoint(touchOne.position)))
            {
                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                float prevDis = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float currDis = (touchZero.position - touchOne.position).magnitude;

                float diff = currDis - prevDis;

                Zoom(diff * 0.1f);
            }
        }
    }

    //for desktops
    private void UpdateDesktop()
    {
        //true only the first time the mouse is touched down
        if (Input.GetMouseButtonDown(0))
        {
            touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        //for draggin camera around (panning) Later also needs to check if they are on any objects
        if (Util.OnCanvas(Camera.main.WorldToScreenPoint(touchStart)) && Input.GetMouseButton(0))
        {
            Vector3 direction = touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Camera.main.transform.position += direction;
        }

        Zoom(Input.mouseScrollDelta.y);
    }

    void Zoom (float increment)
    {
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - increment, zoomMin, zoomMax);
    }
}
