using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SpringControl : MonoBehaviour
{
    private Transform trans;
    private Collider2D col;
    private Rigidbody2D rb;

    private RectRotatePoint _endPoint1;
    private RectRotatePoint _endPoint2;

    private RectRotatePoint endPoint1 //positive/above
    {
        get { return (_endPoint1 != null && _endPoint1.gameObject == null) ? null : _endPoint1; }
        set { _endPoint1 = value; }
    }
    private RectRotatePoint endPoint2 //negative/below
    {
        get { return (_endPoint2 != null && _endPoint2.gameObject == null) ? null : _endPoint2; }
        set { _endPoint2 = value; }
    } 

    public Vector3 attachPoint1;
    public Vector3 attachPoint2;

   // private DistanceJoint2D[] joints = new DistanceJoint2D[2];

    private SpringJoint2D [] joints = new SpringJoint2D [2];

    private Vector3 touchStart;
    private bool onSprite;

    private RectRotatePoint tentative1 = null;
    private RectRotatePoint tentative2 = null;

    private int lastTouchCnt = -1;

    private bool touchAbove;

    private static float heightMultiplier = 1f;

    private float height { get { return heightMultiplier * trans.localScale.y; }}

    public static float defaultHeight = 3f;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void Awake()
    {
        trans = GetComponent<Transform>();
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
    }

    void updateConfigurations ()
    {
        if (onSprite && PropertiesEditable.focusedObject == this.gameObject) this.setConfig(attachPoint1, attachPoint2);
        else if (endPoint1 != null && endPoint2 != null)
        {
            attachPoint1 = endPoint1.getWorldPoint();
            attachPoint2 = endPoint2.getWorldPoint();
            this.setConfig(attachPoint1, attachPoint2);
        }
        else if (!ReplayControl.touchable && joints[0] == null && joints[1] == null) { Debug.Log("sdfd"); this.gameObject.GetComponent<Destructable>().Destruct(); }
        else if (endPoint1 != null)
        {
            Vector3 worldpoint = endPoint1.getWorldPoint();
            attachPoint2 = worldpoint + attachPoint2 - attachPoint1;
            attachPoint1 = worldpoint;
            this.setConfig(attachPoint1, attachPoint2);
        }
        else if (endPoint2 != null)
        {
            Vector3 worldpoint = endPoint2.getWorldPoint();
            attachPoint1 = worldpoint + attachPoint1 - attachPoint2;
            attachPoint2 = worldpoint;
            this.setConfig(attachPoint1, attachPoint2);
        }
        else if (!ReplayControl.touchable) this.gameObject.GetComponent<Destructable>().Destruct();
    }

    public void setConfig(Vector3 position1, Vector3 position2)
    { 
        Vector3 displacement = position1 - position2;
        Vector3 scale = new Vector3();
        float angle = Mathf.Abs(displacement.x) < Util.EPSILON ? ((displacement.y > 0) ? 90f : -90f) : Util.GetAngleFromVectorFloat(displacement);
        scale.z = 0f;
        scale.x = this.trans.localScale.x;
        float distance = Vector3.Distance(position2, position1);
        scale.y = distance / heightMultiplier;
        this.trans.localScale = scale;
        Vector3 newpos = (position1 + position2) / 2f;
        newpos.z = 0;
        this.trans.position = newpos;
        float newangle = (angle - 90f + 720f) % 360f;
        this.trans.localEulerAngles = new Vector3(0f, 0f, newangle);
    }

    void disableEndPoint1 ()
    {
        if (joints[0] != null)
        {
            SpringJoint2D temp = joints[0];
            if (temp != null) Destroy(temp);
            joints[0] = null;
            endPoint1 = null;
            if (joints[1] != null)
            {
                joints[1].enabled = false;
            }
        }
    }

    void disableEndPoint2 ()
    {
        if (joints[1] != null)
        {
            SpringJoint2D temp = joints[1];
            if (temp != null) Destroy(temp);
            joints[1] = null;
            endPoint2 = null;
            if (joints[0] != null)
            {
                joints[0].enabled = false;
            }
        }
    }

    //check if attachable
    void setEndPoint1(GameObject go, Vector2 translate)
    {
        if (endPoint1 != null) Destroy(endPoint1.gameObject.GetComponent<SpringJoint2D>());
        SpringJoint2D joint = go.AddComponent<SpringJoint2D>();
        joint.enabled = false;
        joints[0] = joint;
        endPoint1 = new RectRotatePoint(go, translate);
        //sets the anchor on the current object
        Collider2D tempcol = go.GetComponent<Collider2D>();
        (float, float) size = (tempcol.bounds.size.x, tempcol.bounds.size.y);
        Attachable attach = go.GetComponent<Attachable>();
        joint.anchor = new Vector2 (translate.x / size.Item1 * attach.widthMultiplier, translate.y / size.Item2 * attach.heightMultiplier);
        if (endPoint2 != null)
        {
            joint.connectedBody = endPoint2.gameObject.GetComponent<Rigidbody2D>();
            SpringJoint2D joint2 = endPoint2.gameObject.GetComponent<SpringJoint2D>();
            joint2.enabled = false;
            joint.enabled = true;
            joint2.connectedBody = go.GetComponent<Rigidbody2D>();
            joints[1] = joint2;
            //sets the anchor on the connected object;
            Collider2D tempcol2 = endPoint2.gameObject.GetComponent<Collider2D>();
            (float, float) size2 = (tempcol2.bounds.size.x, tempcol2.bounds.size.y);
            Attachable attach2 = endPoint2.gameObject.GetComponent<Attachable>();
            joint.connectedAnchor = new Vector2(endPoint2.translate.x / size2.Item1 * attach2.widthMultiplier, endPoint2.translate.y / size2.Item2 * attach2.heightMultiplier);
        }
        configureJoints();
    }

    void setEndPoint2 (GameObject go, Vector2 translate)
    {
        if (endPoint2 != null) Destroy(endPoint2.gameObject.GetComponent<SpringJoint2D>());
        SpringJoint2D joint = go.AddComponent<SpringJoint2D>();
        joint.enabled = false;
        joints[1] = joint;
        endPoint2 = new RectRotatePoint(go, translate);
        //sets the anchor on the current object;
        Collider2D tempcol = go.GetComponent<Collider2D>();
        (float, float) size = (tempcol.bounds.size.x, tempcol.bounds.size.y);
        Attachable attach = go.GetComponent<Attachable>();
        joint.anchor = new Vector2(translate.x / size.Item1 * attach.widthMultiplier, translate.y / size.Item2 * attach.heightMultiplier);
        if (endPoint1 != null)
        {
            joint.connectedBody = endPoint1.gameObject.GetComponent<Rigidbody2D>();
            SpringJoint2D joint2 = endPoint1.gameObject.GetComponent<SpringJoint2D>();
            joint2.enabled = false;
            joint.enabled = true;
            joint2.connectedBody = go.GetComponent<Rigidbody2D>();
            joints[0] = joint2;
            //sets the anchor on the connected object;
            Collider2D tempcol2 = endPoint1.gameObject.GetComponent<Collider2D>();
            (float, float) size2 = (tempcol2.bounds.size.x, tempcol2.bounds.size.y);
            Attachable attach2 = endPoint1.gameObject.GetComponent<Attachable>();
            joint.connectedAnchor = new Vector2(endPoint1.translate.x / size2.Item1 * attach2.widthMultiplier, endPoint1.translate.y / size2.Item2 * attach2.heightMultiplier);
        }
        configureJoints();
    }

    //do things to configure the joints
    void configureJoints()
    {
        for (int x = 0; x < 2; x++)
        {
            SpringJoint2D joint = joints[x];
            if (joint != null)
            {
                joint.autoConfigureConnectedAnchor = false;
                joint.autoConfigureDistance = false;
                joint.frequency = 1f;
                joint.dampingRatio = 0f;
                joint.enableCollision = true;
                joint.distance = this.elength;
                joint.breakForce = Mathf.Infinity;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckTouches();
        updateConfigurations();
    }

    void CheckTouches()
    {
        if (!ReplayControl.touchable)
        {
            col.enabled = false;
            return;
        }
        col.enabled = true;
        if (Input.GetMouseButtonDown(0))
        {
            if (Input.touchCount == 1)
            {
                touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                touchStart.z = 0;
                if (col.bounds.Contains(touchStart))
                {
                    onSprite = true;
                    PropertiesEditable.focusedObject = this.gameObject;
                    Util.objectDragged = true;
                    touchAbove = Above(touchStart);
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
                HandleTouch(touch);
                FindNearest();
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (onSprite) UpdateEndPoints();
                onSprite = false;
                Util.objectDragged = false;
            }
            lastTouchCnt = 1;
        }
        else if (Input.touchCount == 2 && onSprite && PropertiesEditable.focusedObject == this.gameObject)
        {
            if (endPoint1 != null || endPoint2 != null) return;

            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector3 pos1 = Camera.main.ScreenToWorldPoint(touchZero.position);
            Vector3 pos2 = Camera.main.ScreenToWorldPoint(touchOne.position);

            Vector3 pos1prev = Camera.main.ScreenToWorldPoint(touchZero.position - touchZero.deltaPosition);
            Vector3 pos2prev = Camera.main.ScreenToWorldPoint(touchOne.position - touchOne.deltaPosition);

            if (touchAbove)
            {
                attachPoint1 += pos1 - pos1prev;
                attachPoint2 += pos2 - pos2prev;
            }
            else
            {
                attachPoint2 += pos1 - pos1prev;
                attachPoint1 += pos2 - pos2prev;
            }

            lastTouchCnt = 2;
        }
    }

    private bool Above(Vector3 touch)
    {
        Vector3 pos = trans.position;
        float angle = trans.eulerAngles.z;
        Vector3 diff = touch - pos;
        float currangle = Mathf.Abs(diff.x) < Util.EPSILON ? (diff.y > 0f ? 90f : -90f) : Util.GetAngleFromVectorFloat(diff);
        currangle -= angle;
        currangle = (currangle + 720f) % 360f;
        return 0 < currangle && currangle < 180;
    }

    private void Move(Vector3 touch)
    {
        attachPoint1 += touch - touchStart;
        attachPoint2 += touch - touchStart;
        touchStart = touch;
    }

    void HandleTouch (Vector3 touch)
    {
        if (endPoint1 == null && endPoint2 == null) Move(touch);
        else if (endPoint1 == null)
        {
            if (touchAbove) attachPoint1 = touch;
            else Move(touch);
        }
        else if (endPoint2 == null)
        {
            if (touchAbove) Move(touch);
            else attachPoint2 = touch;
        }
        else
        {
            if (touchAbove) attachPoint1 = touch;
            else attachPoint2 = touch;
        }
    }

    void UpdateEndPoints()
    {
        if (touchAbove)
        {
            if (tentative1 == null) disableEndPoint1();
            else
            {
                GameObject go = tentative1.gameObject;
                Vector3 worldpoint = go.transform.position + Util.RotateAroundOrigin(tentative1.translate, go.transform.localEulerAngles.z);
                if (endPoint2 == null) attachPoint2 += worldpoint - attachPoint1;
                attachPoint1 = worldpoint;
                setEndPoint1(tentative1.gameObject, tentative1.translate);
            }
        }
        else
        {
            if (tentative2 == null) disableEndPoint2();
            else
            {
                GameObject go = tentative2.gameObject;
                Vector3 worldpoint = go.transform.position + Util.RotateAroundOrigin(tentative2.translate, go.transform.localEulerAngles.z);
                if (endPoint1 == null) attachPoint1 += worldpoint - attachPoint2;
                attachPoint2 = worldpoint;
                setEndPoint2(tentative2.gameObject, tentative2.translate);
            }
        }
        Attachable.focused = null;
    }

    void FindNearest ()
    {
        RaycastHit2D hit;
        int layerMask = (1 << 0); //default layer
        if (touchAbove)
        {
            hit = Physics2D.Raycast(attachPoint1, trans.TransformDirection(Vector3.up), height/2, layerMask);
            Debug.DrawRay(attachPoint1, trans.TransformDirection(Vector3.up)*height/2, Color.green);  // draw the debug;
            if (hit.collider != null)
            {
                Transform gotrans = hit.collider.gameObject.GetComponent<Transform>();
                Vector3 translate = Util.RotateAroundOrigin(new Vector3(hit.point.x, hit.point.y) - gotrans.position, -gotrans.eulerAngles.z);
                SetTentative1(hit.collider.gameObject, translate);
            }
            else DismissTentative1();
        }
        else
        {
            hit = Physics2D.Raycast(attachPoint2, trans.TransformDirection(Vector3.down), height/2, layerMask);
            Debug.DrawRay(attachPoint2, trans.TransformDirection(Vector3.down)*height/2, Color.green);
            if (hit.collider != null)
            {
                Transform gotrans = hit.collider.gameObject.GetComponent<Transform>();
                Vector3 translate = Util.RotateAroundOrigin(new Vector3(hit.point.x, hit.point.y) - gotrans.position, -gotrans.eulerAngles.z);
                SetTentative2(hit.collider.gameObject, translate);
            }
            else DismissTentative2();
        }
    }

    void DismissTentative1()
    {
        Attachable.focused = null;
        if (tentative1 == null) return;
        tentative1 = null;
    }

    void DismissTentative2 ()
    {
        Attachable.focused = null;
        if (tentative2 == null) return;
        tentative2 = null;
    }

    void SetTentative1 (GameObject go, Vector2 translate)
    {
        if (tentative2 != null && tentative2.gameObject == go) return;
        Attachable attach = go.GetComponent<Attachable>();
        if (attach == null) return;
        Attachable.focused = attach;
        tentative1 = new RectRotatePoint(go, translate);
    }

    void SetTentative2(GameObject go, Vector2 translate)
    {
        if (tentative1 != null && tentative1.gameObject == go) return;
        Attachable attach = go.GetComponent<Attachable>();
        if (attach == null) return;
        Attachable.focused = attach;
        tentative2 = new RectRotatePoint(go, translate);
    }

    private void OnJointBreak2D(Joint2D joint)
    {
        Debug.Log(joint.reactionForce);

        if (joint == joints[0] || joint == joints[1])
        {
            disableEndPoint1();
            disableEndPoint2();
        }
    }

    private void FixedUpdate()
    {
        if (ReplayControl.touchable) return;
        for (int i = 0; i < 2; i++)
        {
            SpringJoint2D joint = joints[i];
            if (joint != null && joint.enabled)
            {
                double x = this.getSpringLength() - this.elength;
                double k = this.springConstant;

                Vector3 dirup = this.trans.TransformDirection(Vector3.up).normalized;
                Vector3 dirdown = this.trans.TransformDirection(Vector3.down).normalized;

                Vector2 reactionForce = joint.reactionForce;

                if (endPoint1.gameObject == joint.attachedRigidbody.gameObject)
                {
                    joint.attachedRigidbody.AddForce(reactionForce);
                    joint.connectedBody.AddForce(-1f*reactionForce);

                    joint.attachedRigidbody.AddForce((float)(-k * x) * dirup, ForceMode2D.Force);
                    joint.connectedBody.AddForce((float)(-k * x) * dirdown, ForceMode2D.Force);
                }
                else if (endPoint1.gameObject == joint.connectedBody.gameObject)
                {
                    joint.attachedRigidbody.AddForce(reactionForce);
                    joint.connectedBody.AddForce(-1f*reactionForce);

                    joint.connectedBody.AddForce((float)(-k * x) * dirup, ForceMode2D.Force);
                    joint.attachedRigidbody.AddForce((float)(-k * x) * dirdown, ForceMode2D.Force);
                }
            }
        }
    }

    //MARK: Properties Control
    private float springConstant = 2f; //N/m
    private float breakforce = 50f; //N
    private float elength = 4f; //metres

    public void setSpringConstant (float constant) { this.springConstant = constant; }
    public float getSpringConstant () { return this.springConstant; }

    public double getSpringLength()
    {
        Vector3 diff = attachPoint2 - attachPoint1;
        double xdir = diff.x;
        double ydir = diff.y;
        return Math.Sqrt(xdir * xdir + ydir * ydir);
    }

    public void setBreakForce(float breakforce) { this.breakforce = breakforce; configureJoints(); }
    public float getBreakForce() { return this.breakforce; }
    public void setElength(float elength) { this.elength = elength; configureJoints(); }
    public float getElength () { return this.elength; }
    public bool available () { return endPoint1 == null && endPoint2 == null;} 
}