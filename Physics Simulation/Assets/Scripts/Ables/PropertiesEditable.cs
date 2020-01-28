using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using v2 = UnityEngine.Vector2;
using v3 = UnityEngine.Vector3;
using go = UnityEngine.GameObject;

public class PropertiesEditable: MonoBehaviour
{

    public static go focusedObject;

    public RectTransform canvas;

    private List<go> displays = new List<go>();

    private RectTransform dashTemplate;
    private RectTransform labelTemplate;

    RectTransform dashhorizontal;
    RectTransform dashmoving;
    RectTransform label;

    private Transform trans;

    //FixedRectangle
    

    //MoveableRectangle

    //Circle

    //Velocity

    //Forces

    //Spring

    void Update()
    {
        if (focusedObject != this.gameObject)
        {
            Remove();
            return;
        }
        Name name = this.gameObject.GetComponent<Name>();
        if (name.objectname == "Spring") updateSpring();
        else if (name.objectname == "Force") updateForce();
        else if (name.objectname == "Velocity") updateVelocity();
        else if (name.objectname == "MoveableRectangle") updateMoveableRectangle();
        else if (name.objectname == "FixedRectangle") updateFixedRectangle();
        else if (name.objectname == "Circle") updateCircle();
    }

    private void Remove()
    {
        foreach (go each in displays) each.SetActive(false);
    }

    void updateSpring()
    {
        
    }

    void updateForce()
    {

    }

    void updateVelocity()
    {

    }

    void updateMoveableRectangle()
    {
        updateRect();
    }

    void updateFixedRectangle()
    {
        updateRect();
    }

    void updateRect()
    {
        dashhorizontal.gameObject.SetActive(true);
        dashmoving.gameObject.SetActive(true);
        label.gameObject.SetActive(true);

        //this needs angle, length and width;
        v3 pos = trans.position;
        pos.z = 0;
        v3 localpos = Camera.main.WorldToScreenPoint(pos)/this.canvas.localScale.x;
        localpos.z = 0;

        //set position and angle
        dashhorizontal.anchoredPosition = localpos;
        dashhorizontal.eulerAngles = v3.zero;

        //dashmoving.anchoredPosition = localpos;
        dashmoving.anchoredPosition = localpos;
        dashmoving.eulerAngles = trans.eulerAngles;

        //set width
        float length = trans.localScale.x * Util.FixedRectWidthMultiplier * Util.WorldToScreenMultiplier / 2f;
        dashmoving.sizeDelta = dashhorizontal.sizeDelta = new v2(length, 3f);

        //set the label
        //get angle
        Text t = label.GetComponent<Text>();
        float angle = trans.eulerAngles.z;
        t.text = Mathf.Round(angle*100f)/100f + "°";

        if (angle > 30f) angle += 15;
        else angle /= 2f;
        v3 direction = new v3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle)).normalized;
        length = 0f;
        label.anchoredPosition = localpos + direction * length;
    }

    void updateCircle()
    {

    }

    private void Start()
    {

    }

    public void Destruct()
    {
        Measurement m = this.gameObject.GetComponent<Measurement>();
        if (m != null) m.Destruct();
        foreach (go each in displays) Destroy(each);
    }

    private void Awake()
    {
        this.canvas = go.Find("Canvas").GetComponent<RectTransform>();

        go obj = new go("line", typeof(Image));
        obj.transform.SetParent(this.canvas, false);
        obj.GetComponent<Image>().color = new Color(1, 1, 1);
        dashhorizontal = obj.GetComponent<RectTransform>();
        dashhorizontal.anchorMax = dashhorizontal.anchorMin = new v2(0f, 0f);
        dashhorizontal.pivot = new v2(0f, 0.5f);
        dashhorizontal.gameObject.SetActive(false);
        displays.Add(obj);

        go obj2 = new go("line", typeof(Image));
        obj2.transform.SetParent(this.canvas, false);
        obj2.GetComponent<Image>().color = new Color(1, 1, 1);
        dashmoving = obj2.GetComponent<RectTransform>();
        dashmoving.anchorMax = dashmoving.anchorMin = new v2(0f, 0f);
        dashmoving.pivot = new v2(0f, 0.5f);
        dashmoving.gameObject.SetActive(false);
        displays.Add(obj2);

        go obj3 = new go("angleText", typeof(Text));
        obj3.transform.SetParent(this.canvas, false);
        label = obj3.GetComponent<RectTransform>();
        label.anchorMin = label.anchorMax = new v2(0f, 0f);
        label.pivot = new v2(0f, 0f);
        label.gameObject.SetActive(false);
        displays.Add(obj3);
        Text t = label.GetComponent<Text>();
        t.alignment = TextAnchor.MiddleCenter;
        t.color = new Color(1, 1, 1);
        t.fontSize = 30;
        t.font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");

        trans = this.gameObject.GetComponent<Transform>();
        
    }
}
