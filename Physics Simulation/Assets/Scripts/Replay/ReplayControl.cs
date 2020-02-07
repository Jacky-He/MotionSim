using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ReplayControl : MonoBehaviour
{
    public string spriteName;
    //replay
    public static bool sliderReplaying = false;
    public static bool resetting = false;
    public static bool recording = false;
    public static bool replaying = false;
    public static bool touchable = true;

    public static long totalPointsCnt = 0;

    public static bool controlledByAnim = true;
    public static bool adjustable = true;

    public static int helperCnt = 0;

    public int globalIndex = 0;

    public List<PointInTime> pointsInTime;

    Vector2 prevVelocity = new Vector2(0f, 0f);

    //for graphing
    private static GameObject canvassObject = null;
    public static RectTransform graphWindow = null;
    public static GraphControl gc = null;

    //this is the object that is currently focused;
    public static GameObject focusedObject;

    public GraphOptions graphOption;

    public HashSet<GraphOptions> graphOptions = new HashSet<GraphOptions>();

    private Rigidbody2D rb;

    private Transform spriteTransform;

    private Collider2D col;

    //for dragging
    private Vector3 touchStart;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        if (totalPointsCnt > 138000) ButtonsControl.StaticStop();
        //this object will be destroyed
        if (spriteName != "FixedRectangle" && spriteName != "Spring" && spriteName != "Force")
        {
            rb.isKinematic = controlledByAnim;
        }
        if (resetting) Reset();
        if (sliderReplaying) Adjust();
        if (recording) Record();
        if (replaying) Replay();
    }

    private void Adjust()
    {
        if (pointsInTime.Count == 0) return;
        globalIndex = 0; //so that when the user presses stop, the objects are set to their initial positions (if it was replaying)
        float percentage = SliderControl.percentage;
        int index = Mathf.RoundToInt(percentage * (pointsInTime.Count - 1)); //shouldn't overflow;
        PointInTime p = pointsInTime[index];
        this.transform.position = initPos + p.position;
        this.transform.rotation = p.rotation;
        if (focusedObject == this.gameObject) gc.ShowGraph(pointsInTime, graphOptions, index);
    }

    Vector3 initPos = Vector3.zero;
    bool initpospresent = false;

    private void Record ()
    {
        if (!graphWindow.gameObject.activeSelf) graphWindow.gameObject.SetActive(true);
        //adds the velocity and acceleration of the objects
        Vector2 currAcceleration = (rb.velocity - prevVelocity) / Time.fixedDeltaTime;
        prevVelocity = rb.velocity;

        if (!initpospresent) { initPos = this.transform.position; initpospresent = true; }

        pointsInTime.Add(new PointInTime(this.transform.position - initPos, this.transform.rotation, rb.velocity, rb.angularVelocity, currAcceleration));
        totalPointsCnt++;
        if (focusedObject == this.gameObject) gc.ShowGraph(pointsInTime, graphOptions);
    }

    private void Reset()
    {
        if (pointsInTime.Count > 0)
        {
            PointInTime init = pointsInTime[0];
            this.transform.position = initPos + init.position;
            //Debug.Log(this.transform.position);
            this.transform.rotation = init.rotation;
            rb.velocity = init.velocity;
            rb.angularVelocity = init.angularVelocity;
            pointsInTime.Clear();
            initpospresent = false;
            initPos = Vector3.zero;
        }
        if (focusedObject == this.gameObject)
        {
            gc.ShowGraph(pointsInTime, graphOptions); // this should show an empty graph;
        }
    }

    private void Replay()
    {
        if (globalIndex < pointsInTime.Count)
        {
            PointInTime curr = pointsInTime[globalIndex];
            this.transform.position = initPos + curr.position;
            this.transform.rotation = curr.rotation;
            globalIndex++;
        }
        else
        {
            globalIndex = 0;
        }
        if (focusedObject == this.gameObject)
        {
            gc.ShowGraph(this.pointsInTime, graphOptions, globalIndex);
        }
    }

    

    // Update is called once per frame
    void Update()
    {

    }

    //when floating point precision issues start to occur
    private void Awake()
    {
        //temporary
        this.graphOption = GraphOptions.velocityX;
        rb = this.GetComponent<Rigidbody2D>();
        spriteTransform = this.GetComponent<Transform>();
        pointsInTime = new List<PointInTime>();
        this.col = this.GetComponent<Collider2D>();
        if (spriteName != "FixedRectangle" && spriteName != "Spring" && spriteName != "Force") rb.isKinematic = controlledByAnim;
        else rb.isKinematic = true;
        helperCnt++;
        if (canvassObject == null) canvassObject = GameObject.Find("Canvas");
    }
}