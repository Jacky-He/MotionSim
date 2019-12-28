﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringControl : MonoBehaviour
{
    private Transform trans;
    private Collider2D col;
    private Rigidbody2D rb;

    private RectRotatePoint endPoint1; //positive/above
    private RectRotatePoint endPoint2; //negative/below

    private Vector3 attachPoint1;
    private Vector3 attachPoint2;

    private SpringJoint2D firstJoint;
    private SpringJoint2D secondJoint;

    private Vector3 touchStart;
    private bool onSprite;

    private Vector2 attachPoint;
    private Transform nearestTrans;

    private static float heightMultiplier = 1f;

    private float height { get { return heightMultiplier * trans.localScale.y; }}

    //temporary
    [SerializeField]
    private GameObject obj1;
    [SerializeField]
    private GameObject obj2;

    // Start is called before the first frame update
    void Start()
    {
        setEndPoint1(obj1, new Vector2(0f, 0f));
        setEndPoint2(obj2, new Vector2(0f, 0f));
    }

    private void Awake()
    {
        trans = GetComponent<Transform>();
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    void updateConfigurations ()
    {
        if (endPoint1 != null && endPoint2 != null)
        {
            RectRotatePoint.setConfig(trans, endPoint1, endPoint2);
        }
        else if (endPoint1 != null)
        {
            RectRotatePoint.setConfig(trans, endPoint1);
        }
        else if (endPoint2 != null)
        {
            RectRotatePoint.setConfig(trans, endPoint2);
        }
    }

    void disableEndPoint1 ()
    {
        if (firstJoint != null)
        {
            firstJoint.enabled = false;
            firstJoint = null;
            if (secondJoint != null)
            {
                secondJoint.enabled = false;
            }
        }
    }

    void disableEndPoint2 ()
    {
        if (secondJoint != null)
        {
            secondJoint.enabled = false;
            secondJoint = null;
            if (firstJoint != null)
            {
                firstJoint.enabled = false;
            }
        }
    }

    void setEndPoint1(GameObject go, Vector2 translate)
    {
        if (endPoint1 != null) endPoint1.gameObject.GetComponent<SpringJoint2D>().enabled = false;
        endPoint1 = new RectRotatePoint(go, translate);
        SpringJoint2D joint = go.GetComponent<SpringJoint2D>();
        joint.enabled = true;
        firstJoint = joint;
        if (endPoint2 != null)
        {
            joint.connectedBody = endPoint2.gameObject.GetComponent<Rigidbody2D>();
            SpringJoint2D joint2 = endPoint2.gameObject.GetComponent<SpringJoint2D>();
            joint2.enabled = true;
            joint2.connectedBody = go.GetComponent<Rigidbody2D>();
            secondJoint = joint2;
        }
    }

    void setEndPoint2 (GameObject go, Vector2 translate)
    {
        if (endPoint2 != null)
        {
            endPoint2.gameObject.GetComponent<SpringJoint2D>().enabled = false;
        }
        endPoint2 = new RectRotatePoint(go, translate);
        SpringJoint2D joint = go.GetComponent<SpringJoint2D>();
        joint.enabled = true;
        secondJoint = joint;
        if (endPoint1 != null)
        {
            joint.connectedBody = endPoint1.gameObject.GetComponent<Rigidbody2D>();
            SpringJoint2D joint2 = endPoint1.gameObject.GetComponent<SpringJoint2D>();
            joint2.enabled = true;
            joint2.connectedBody = go.GetComponent<Rigidbody2D>();
            firstJoint = joint2;
        }
    }

    // Update is called once per frame
    void Update()
    {
        updateConfigurations();
        if (endPoint1 == null || endPoint2 == null)
        {
            CheckTouches();
        }
    }

    void CheckTouches()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Input.touchCount == 1)
            {
                touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                touchStart.z = 0;
                if (col.bounds.Contains(touchStart))
                {
                    onSprite = true;
                    touchStart.z = 0;
                    Util.objectDragged = true;
                }
            }
        }
        if (Input.touchCount == 1)
        {
            if (!onSprite) return;
            if (Input.GetMouseButton(0))
            {
                Vector3 touch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                touch.z = 0;
                HandleTouch(touch);
                FindNearest(touch);
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (onSprite) SnapIn();
                onSprite = false;
                Util.objectDragged = false;
            }
        }
    }

    void HandleTouch (Vector3 touch)
    {
        if (endPoint1 == null && endPoint2 == null)
        {
            trans.position += touch - touchStart;
            touchStart = touch;
        }
        else if (endPoint1 == null)
        {

        }
        else if (endPoint2 == null)
        {

        }
        else
        {

        }
    }

    void SnapIn()
    {
        
    }

    void FindNearest (Vector3 touch)
    {
        Vector3 pos = trans.position;
        float angle = trans.eulerAngles.z;
        Vector3 diff = touch - pos;
        float currangle = diff.x < Util.EPSILON ? (diff.y > 0f ? 90f : -90f) : Util.GetAngleFromVectorFloat(diff);
        currangle -= angle;
        currangle = (currangle + 720f)% 360f;
        if (0 < currangle && currangle < 180) //above
        {
            attachPoint.x = pos.x + height / 2f * Mathf.Cos(Mathf.Deg2Rad * angle);
            attachPoint.y = pos.y + height / 2f * Mathf.Sin(Mathf.Deg2Rad * angle);
        }
        else //below
        {
            attachPoint.x = pos.x + height / 2f * Mathf.Cos(Mathf.Deg2Rad * (angle + 180f));
            attachPoint.y = pos.y + height / 2f * Mathf.Sin(Mathf.Deg2Rad * (angle + 180f));
        }
        nearestTrans = Attachable.tree.NearestNeighbour(attachPoint);
        if (nearestTrans == null) return;
        Attachable attach = nearestTrans.gameObject.GetComponent<Attachable>();
        attach.ShowSelected();
    }
}
