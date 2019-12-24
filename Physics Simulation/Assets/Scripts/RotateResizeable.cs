using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateResizeable : MonoBehaviour
{
    private Collider2D col;
    private Vector3 touchStart;
    private Transform trans;
    private bool onSprite = false;
    private int lastTouchCnt = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        col = this.GetComponent<Collider2D>();
        trans = this.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown (0))
        {
            if (Input.touchCount == 1)
            {
                touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                touchStart.z = 0;
                if (col.bounds.Contains(touchStart))
                {
                    Debug.Log("sdfds");
                    onSprite = true;
                    touchStart.z = 0;
                    Util.objectDragged = true;
                    ReplayControl.focusedObject = this.gameObject;
                }
            }
        }
        if (Input.touchCount == 1)
        {
            if (!onSprite) return;
            if (lastTouchCnt == 2) touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (Input.GetMouseButton(0))
            {
                Vector3 touch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                touch.z = 0;

                trans.position += touch - touchStart;
                touchStart = touch;
            }
            if (Input.GetMouseButtonUp(0))
            {
                onSprite = false;
                Util.objectDragged = false;
            }
            lastTouchCnt = 1;
        }
        else if (Input.touchCount == 2 && onSprite)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector3 pos1 = Camera.main.ScreenToWorldPoint(touchZero.position);
            Vector3 pos2 = Camera.main.ScreenToWorldPoint(touchOne.position);

            Vector3 pos1prev = Camera.main.ScreenToWorldPoint(touchZero.position - touchZero.deltaPosition);
            Vector3 pos2prev = Camera.main.ScreenToWorldPoint(touchOne.position - touchOne.deltaPosition);

            float angle = Util.GetAngleFromVectorFloat(pos2 - pos1);
            float prevangle = Util.GetAngleFromVectorFloat(pos2prev - pos1prev);
            float angledelta = angle - prevangle;

            transform.eulerAngles += new Vector3(0, 0, angledelta);

            Vector3 averagePosPrev = (pos2prev + pos1prev) / 2f;
            Vector3 averagePos = (pos1 + pos2) / 2f;

            transform.position += (averagePos - averagePosPrev);
            lastTouchCnt = 2;
        }
    }
}
