using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ObjectDropHandler : MonoBehaviour, IDropHandler
{
    private Vector3 touchStart;
    private float zoomMax = 1000;
    private float zoomMin = 40;

    public void OnDrop (PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;
        Vector3 pos = eventData.position;
        //check if pos is inside of scrollViewRect
        if (Util.OnCanvas(pos))
        {
            //this is the object being dropped
            //instantiate a new object in the canvass
            GameObject ui = eventData.pointerDrag;
            GameObject reference = ui.GetComponent<ObjectDragHandler>().spritePreFab;
            GameObject newObj = Instantiate(reference) as GameObject;
            newObj.SetActive(true);
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
            worldPos.z = 0;
            newObj.transform.position = worldPos;
        }
        //otherwise do nothing;
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        Input.multiTouchEnabled = true;
        Camera.main.orthographicSize = 257f;
        Camera.main.transform.position = new Vector3(456, 256.5f, -10);
    }

    private void UpdateMobile()
    {
        if (Input.touchCount == 1)
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

        }
        else if (Input.touchCount == 2)
        {
            //zooming camera
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            //if both touches are not in the scrollview;
            if (Util.OnCanvas(Camera.main.WorldToScreenPoint(touchZero.position)) && Util.OnCanvas(Camera.main.WorldToScreenPoint(touchOne.position)))
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
        Zoom(Input.mouseScrollDelta.y);
    }

    void Zoom(float increment)
    {
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - increment, zoomMin, zoomMax);
    }
}
