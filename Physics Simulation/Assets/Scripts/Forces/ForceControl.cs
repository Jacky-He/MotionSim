using UnityEngine;
using System.Collections;

public class ForceControl: MonoBehaviour
{

    private RectRotatePoint _targetPoint;
    private RectRotatePoint targetPoint
    {
        get { return (_targetPoint != null && _targetPoint.gameObject == null) ? null : _targetPoint; }
        set { _targetPoint = value; }
    }
    private RectRotatePoint tentative;
    private Force force = new Force ();

    private Collider2D col;
    private Transform trans;

    private Vector3 touchStart;
    private bool onSprite;
    private int lastTouchCnt = -1;

    public Vector3 attachPoint1;   //this is the arrowhead
    public Vector3 attachPoint2;   //this is the arrowfeet
    private bool touchAbove;

    private static float heightMultiplier = 1f;
    public static float defaultHeight = 1f;

    private float height { get { return heightMultiplier * trans.localScale.y; } }

    private void Awake()
    {
        trans = this.GetComponent<Transform>();
        col = this.GetComponent<Collider2D>();
    }

    // Use this for initialization
    void Start()
    {

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

    private void CheckTouches()
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
                if (onSprite) UpdateTargetPoint();
                onSprite = false;
                Util.objectDragged = false;
            }
            lastTouchCnt = 1;
        }
        else if (Input.touchCount == 2 && onSprite && PropertiesEditable.focusedObject == this.gameObject)
        {
            if (targetPoint != null) return;

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

    private void Move(Vector3 touch)
    {
        attachPoint1 += touch - touchStart;
        attachPoint2 += touch - touchStart;
        touchStart = touch;
    }

    private void HandleTouch(Vector3 touch)
    {
        if (targetPoint == null) Move(touch);
        else
        {
            if (touchAbove) attachPoint1 = touch;
            else Move(touch);
        }
    }

    private void FindNearest ()
    {
        RaycastHit2D hit;
        int layerMask = (1 << 0); //default layer;
        hit = Physics2D.Raycast(attachPoint2, trans.TransformDirection(Vector3.down), height / 2f, layerMask);
        Debug.DrawRay(attachPoint2, trans.TransformDirection(Vector3.down) * height / 2f, Color.green);
        if (hit.collider != null)
        {
            Transform gotrans = hit.collider.gameObject.GetComponent<Transform>();
            Vector3 translate = Util.RotateAroundOrigin(new Vector3(hit.point.x, hit.point.y) - gotrans.position, -gotrans.eulerAngles.z);
            SetTentative(hit.collider.gameObject, translate);
        }
        else DismissTentative();
    }

    private void SetTentative (GameObject go, Vector3 translate)
    {
        Attachable attach = go.GetComponent<Attachable>();
        if (attach == null) return;
        Attachable.focused = attach;
        tentative = new RectRotatePoint(go, translate);
    }

    private void DismissTentative ()
    {
        Attachable.focused = null;
        if (tentative == null) return;
        tentative = null;
    }

    private void UpdateTargetPoint()
    {
        if (tentative == null) dismissTargetPoint();
        else
        {
            GameObject go = tentative.gameObject;
            Vector3 worldpoint = go.transform.position + Util.RotateAroundOrigin(tentative.translate, go.transform.localEulerAngles.z);
            attachPoint1 += worldpoint - attachPoint2;
            attachPoint2 = worldpoint;
            setTargetPoint(tentative.gameObject, tentative.translate);
        }
        Attachable.focused = null;
    }

    private void updateConfigurations()
    {
        if (ReplayControl.touchable)
        {
            this.gameObject.GetComponent<SpriteRenderer>().enabled = true;
            if (onSprite && PropertiesEditable.focusedObject == this.gameObject) this.setConfig(attachPoint1, attachPoint2);
            else if (targetPoint != null)
            {
                //maybe this should rotate with the object
                Vector3 worldpoint = targetPoint.getWorldPoint();
                attachPoint1 = worldpoint + attachPoint1 - attachPoint2;
                attachPoint2 = worldpoint;
                this.setConfig(attachPoint1, attachPoint2);
            }
        }
        else
        {
            if (targetPoint == null) this.gameObject.GetComponent<Destructable>().Destruct();
            else
            {
                this.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                Vector3 worldpoint = targetPoint.getWorldPoint();
                attachPoint1 = worldpoint + attachPoint1 - attachPoint2;
                attachPoint2 = worldpoint;
                this.setConfig(attachPoint1, attachPoint2);
            }
        }
    }

    private void setConfig (Vector3 position1, Vector3 position2)
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
        this.trans.localEulerAngles = new Vector3 (0f, 0f, newangle);
        //configure the forces
        force.magnitude = distance * 10f;
        force.normalizedDirection = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle)).normalized;
    }

    private void setTargetPoint(GameObject go, Vector3 translate)
    {
        targetPoint = new RectRotatePoint(go, translate);
        Attachable attach = targetPoint.gameObject.GetComponent<Attachable>();
        attach.AddForce(force);
    }

    private void dismissTargetPoint()
    {
        if (targetPoint == null) return;
        Attachable attach = targetPoint.gameObject.GetComponent<Attachable>();
        attach.RemoveForce(force);
        targetPoint = null;
    }

    // Update is called once per frame
    void Update()
    {
        CheckTouches();
        updateConfigurations();
    }
}