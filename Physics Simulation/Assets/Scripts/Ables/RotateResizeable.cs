﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateResizeable : MonoBehaviour
{
    private Collider2D col;
    private Vector3 touchStart;
    private Transform trans;
    private bool onSprite = false;
    private int lastTouchCnt = 0;
    private PropertiesAreaScript propertiesAreaScript;
    public GameObject propertiesArea;

    private float angleprev = -1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        col = this.GetComponent<Collider2D>();
        trans = this.GetComponent<Transform>();
        propertiesAreaScript = propertiesArea.GetComponent<PropertiesAreaScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!(Mathf.Abs(trans.position.z) < Util.EPSILON)) trans.position = new Vector3(trans.position.x, trans.position.y, 0);
        if (Input.GetMouseButtonDown (0))
        {
            if (Input.touchCount == 1)
            {
                Vector3 screenpos = Input.mousePosition;
                screenpos.z = 0;
                touchStart = Camera.main.ScreenToWorldPoint(screenpos);
                touchStart.z = 0;
                if (Util.ColliderContains(col, touchStart, 0) && Util.OnCanvas(screenpos))
                {
                    onSprite = true;
                    PropertiesEditable.focusedObject = this.gameObject;
                    touchStart.z = 0;
                    Util.objectDragged = true;
                    Util.draggedObject = this.gameObject;
                    ReplayControl.focusedObject = this.gameObject;
                }
            }
        }
        if (Input.touchCount == 1 && onSprite && PropertiesEditable.focusedObject == this.gameObject)
        {
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
                Util.draggedObject = null;
            }
            lastTouchCnt = 1;
        }
        else if (Input.touchCount == 2 && onSprite && PropertiesEditable.focusedObject == this.gameObject)
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
            if (!(Mathf.Abs(angleprev - transform.eulerAngles.z) < Util.EPSILON)) { propertiesAreaScript.CoerceAdjustValues(7); angleprev = transform.eulerAngles.z; }

            Vector3 averagePosPrev = (pos2prev + pos1prev) / 2f;
            averagePosPrev.z = 0;
            Vector3 averagePos = (pos1 + pos2) / 2f;
            averagePos.z = 0;

            transform.position += (averagePos - averagePosPrev);
            lastTouchCnt = 2;
        }
    }
}
