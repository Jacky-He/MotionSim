using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringControl : MonoBehaviour
{
    private Transform trans;
    private Collider2D col;
    private Rigidbody2D rb;

    private RectRotatePoint endPoint1;
    private RectRotatePoint endPoint2;

    private SpringJoint2D firstJoint;
    private SpringJoint2D secondJoint;

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

    private void FixedUpdate()
    {
        updateConfigurations();
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
        //updateConfigurations();
    }
}
