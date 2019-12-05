using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplayControl : MonoBehaviour
{

    public static bool sliderReplaying = false;
    public static bool resetting = false;
    public static bool recording = false;

    public  static bool needsClearing = false;

    private static int helperCnt = 0;

    private int globalIndex = 0;

    private List<PointInTime> pointsInTime;

    // Start is called before the first frame update
    void Start()
    {
        pointsInTime = new List<PointInTime>();
        helperCnt++;
    }

    public static bool replaying = false;

    void FixedUpdate()
    {
        //this object will be destroyed
        if (needsClearing) Clear();
        if (resetting) Reset();
        if (sliderReplaying) Adjust();
        if (recording) Record();
        if (replaying) Replay();
    }

    private void Adjust()
    {
        if (pointsInTime.Count == 0) return;
        float percentage = SliderControl.percentage;
        int index = Mathf.RoundToInt(percentage * (pointsInTime.Count - 1)); //shouldn't overflow;
        PointInTime p = pointsInTime[index];
        this.transform.position = p.position;
        this.transform.rotation = p.rotation;
    }

    private void Record ()
    {
        Rigidbody2D rb = this.GetComponent<Rigidbody2D>();
        pointsInTime.Add(new PointInTime(this.transform.position, this.transform.rotation, rb.velocity, rb.angularVelocity));
    }

    private void Reset()
    {
        if (pointsInTime.Count > 0)
        {
            Debug.Log("sdf");
            PointInTime init = pointsInTime[0];
            this.transform.position = init.position;
            this.transform.rotation = init.rotation;
            Rigidbody2D rb = this.GetComponent<Rigidbody2D>();
            rb.velocity = init.velocity;
            rb.angularVelocity = init.angularVelocity;
            pointsInTime.Clear();
        }
    }

    private void Replay()
    {
        if (globalIndex < pointsInTime.Count)
        {
            PointInTime curr = pointsInTime[globalIndex];
            this.transform.position = curr.position;
            this.transform.rotation = curr.rotation;
            globalIndex++;
        }
        else
        {
            globalIndex = 0;
        }
    }

    private void Clear()
    {
        pointsInTime.Clear();
        helperCnt--;
        if (helperCnt == 0) needsClearing = false;
        Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
