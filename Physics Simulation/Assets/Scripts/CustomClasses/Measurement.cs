using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using v2 = UnityEngine.Vector2;
using v3 = UnityEngine.Vector3;
using go = UnityEngine.GameObject;
using rtm = UnityEngine.RectTransform;
using tm = UnityEngine.Transform;

public class Measurement : MonoBehaviour
{
    private float angle;

    private bool isActive = false;

    private static float fixedHeight = 80f;

    private rtm canvas;

    private tm trans;

    private List<rtm>[] rtms = new List<rtm>[2];

    //0: width, 1, height
    //anchor1line: 0, anchor2line: 1, anchor1connectline: 2, anchor2connectline: 3

    private void Initiate()
    {
        for (int i = 0; i < 2; i++) rtms[i] = new List<rtm>();
        canvas = go.Find("Canvas").GetComponent<rtm>();
        for (int j = 0; j < 2; j++)
        {
            for (int i = 0; i < 4; i++)
            {
                go obj = new go("line", typeof(Image));
                obj.transform.SetParent(this.canvas, false);
                obj.GetComponent<Image>().color = new Color(1, 1, 1);
                rtm temp = obj.GetComponent<rtm>();
                temp.anchorMax = temp.anchorMin = new v2(0f, 0f);
                temp.pivot = new v2(0f, 0.5f);
                temp.gameObject.SetActive(isActive);
                rtms[j].Add(temp);
            }
        }

        //adds the label
        for (int i = 0; i < 2; i++)
        {
            go obj3 = new go("lengthText", typeof(Text));
            obj3.transform.SetParent(this.canvas, false);
            rtm label = obj3.GetComponent<rtm>();
            label.anchorMin = label.anchorMax = new v2(0.5f, 0.5f);
            label.pivot = new v2(0.5f, 0.5f);
            label.gameObject.SetActive(false);
            rtms[i].Add(label);
            Text t = label.GetComponent<Text>();
            t.color = new Color(1, 1, 1);
            t.fontSize = 60;
            t.font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        }
    }

    public void Destruct()
    {
        for (int i = 0; i < 2; i++) foreach (rtm each in rtms[i]) Destroy(each.gameObject);
    }

    public void setActive(bool val)
    {
        for (int i = 0; i < 2; i++) foreach (rtm each in rtms[i]) each.gameObject.SetActive(val);
    }

    private void Awake()
    {
        Initiate();
        this.setActive(false);
        this.trans = this.gameObject.GetComponent<tm>();
    }

    private void Update()
    {
        if (PropertiesEditable.focusedObject == this.gameObject) show();
        else unshow();
    }

    // Use this for initialization
    void Start()
    {

    }


    private void show()
    {
        this.setActive(true);
        float angle = trans.eulerAngles.z;
        v3 pos = trans.position;
        pos.z = 0;

        (float, float) dimensions = (trans.localScale.x, trans.localScale.y); //width, height;

        Name name = this.gameObject.GetComponent<Name>();
        if (name.objectname == "FixedRectangle") dimensions = (dimensions.Item1 * Util.FixedRectWidthMultiplier, dimensions.Item2 * Util.FixedRectHeightMultiplier);
        else if (name.objectname == "MoveableRectangle") dimensions = (dimensions.Item1 * Util.MoveableRectWidthMultiplier, dimensions.Item2 * Util.MoveableRectHeightMultiplier);
        else if (name.objectname == "Circle") dimensions = (dimensions.Item1 * Util.CircleDiameterMultiplier, dimensions.Item2 * Util.CircleDiameterMultiplier);
        //facing left; height measurement
        v3 leftdir = new v3(-Mathf.Cos(angle * Mathf.Deg2Rad), -Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;
        v3 leftmiddle = pos + leftdir * dimensions.Item1 / 2f;
        float a2 = angle + 90f;
        v3 updir = new v3(Mathf.Cos(a2 * Mathf.Deg2Rad), Mathf.Sin(a2 * Mathf.Deg2Rad)).normalized;
        v3 anchor1 = leftmiddle + updir * dimensions.Item2 / 2f;
        v3 anchor2 = leftmiddle - updir * dimensions.Item2 / 2f;
        this.update((angle + 180f) % 360f, anchor1, anchor2, 1);
        //facing down;
        v3 upmiddle = pos + updir * dimensions.Item2 / 2f;
        v3 a = upmiddle + leftdir * dimensions.Item1 / 2f;
        v3 b = upmiddle - leftdir * dimensions.Item1 / 2f;
        this.update((angle + 90f) % 360f, a, b, 0);
    }

    private void unshow()
    {
        this.setActive(false);
    }

    public void update(float angle, v3 anchor1, v3 anchor2, int idx) //0: means facing right
    {
        anchor1.z = 0;
        anchor2.z = 0;
        float dislabel = v3.Distance(anchor1, anchor2);
        
        anchor1 = Camera.main.WorldToScreenPoint(anchor1)/this.canvas.localScale.x;
        anchor2 = Camera.main.WorldToScreenPoint(anchor2)/this.canvas.localScale.x;
        anchor1.z = 0;
        anchor2.z = 0;

        v3 middlepos = (anchor1 + anchor2) / 2f;
        float dis = v3.Distance(anchor1, anchor2);

        rtms[idx][0].anchoredPosition = anchor1;
        rtms[idx][1].anchoredPosition = anchor2;

        float a2 = angle + 90f;
        v3 olddir = new v3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0).normalized;
        v3 newdir = new v3(Mathf.Cos(Mathf.Deg2Rad * a2), Mathf.Sin(Mathf.Deg2Rad * a2), 0).normalized;
        rtms[idx][2].anchoredPosition = anchor1 + olddir * fixedHeight/2f;
        rtms[idx][3].anchoredPosition = anchor2 + olddir * fixedHeight/2f;

        rtms[idx][0].sizeDelta = rtms[idx][1].sizeDelta = new v2(fixedHeight, 2f);
        rtms[idx][2].sizeDelta = rtms[idx][3].sizeDelta = new v2(Mathf.Max(0, (dis - 100f) / 2f), 2f);

        rtms[idx][0].eulerAngles = rtms[idx][1].eulerAngles = new v3(0, 0, angle);
        rtms[idx][2].eulerAngles = rtms[idx][3].eulerAngles = new v3(0, 0, angle + 90f);

        v3 testpos = anchor1 + newdir * dis;
        if (Mathf.Abs(v3.Distance(testpos, anchor2)) < Util.EPSILON_SMALL) rtms[idx][3].eulerAngles = new v3(0, 0, rtms[idx][3].eulerAngles.z + 180f);
        else rtms[idx][2].eulerAngles = new v3(0, 0, rtms[idx][2].eulerAngles.z + 180f);

        //text
        rtms[idx][4].position = (rtms[idx][2].position + rtms[idx][3].position) / 2f;
        Text t = rtms[idx][4].GetComponent<Text>();
        t.text = "" + Mathf.Round(dislabel * 100) / 100f + " m";
        t.fontSize = 30;
        t.alignment = TextAnchor.MiddleCenter;
        rtms[idx][4].eulerAngles = new v3(0, 0, 0);
    }
}
