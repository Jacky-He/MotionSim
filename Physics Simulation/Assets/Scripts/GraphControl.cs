using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;

public class GraphControl : MonoBehaviour
{
    [SerializeField] private Sprite circleSprite;
    private RectTransform graphContainer;
    private RectTransform labelTemplateX;
    private RectTransform labelTemplateY;
    private RectTransform dashTemplateX;
    private RectTransform dashTemplateY;
    private List<GameObject> gameObjectList;
    private static int numDataPoints = 25;

    private const float EPSILON = 0.00001f;

    private void Awake()
    {
        graphContainer = this.gameObject.transform.Find("GraphContainer").GetComponent<RectTransform>();
        labelTemplateX = graphContainer.Find("LabelTemplateX").GetComponent<RectTransform>();
        labelTemplateY = graphContainer.Find("LabelTemplateY").GetComponent<RectTransform>();
        dashTemplateX = graphContainer.Find("DashTemplateX").GetComponent<RectTransform>();
        dashTemplateY = graphContainer.Find("DashTemplateY").GetComponent<RectTransform>();
        gameObjectList = new List<GameObject>();
    }

    //makes a circle
    private GameObject CreateCircle (Vector2 anchorPosition)
    {
        GameObject obj = new GameObject("dot", typeof(Image));
        obj.transform.SetParent(graphContainer, false);
        obj.GetComponent<Image>().sprite = circleSprite;
        RectTransform rectTransform = obj.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchorPosition;
        rectTransform.sizeDelta = new Vector2(4f, 4f);
        rectTransform.anchorMax = new Vector2(0f, 0f);
        rectTransform.anchorMin = new Vector2(0f, 0f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        return obj;
    }
            

    public void ShowGraph (List<PointInTime> inputList, GraphOptions option)
    {
        //time adjustment is still needed
        //maybe should take into account for the last object as well?
        List<float> valueList = new List<float>();
        int interval = Math.Max(1, inputList.Count / numDataPoints);
        for (int i = 0, j = 0; i < inputList.Count && j < numDataPoints; i += interval, j++)
        {
            valueList.Add(inputList[i].getOption(option));
        }
        ShowGraph_Helper(valueList, option);
    }

    //overload
    public void ShowGraph (List<PointInTime> inputList, GraphOptions option, int maxIndex)
    {
        //time adjustment is still needed
        //maybe should take into account for the last object as well?
        List<float> valueList = new List<float>();
        int interval = Math.Max(1, (maxIndex + 1) / numDataPoints);
        for (int i = 0, j = 0; i <= maxIndex && j < numDataPoints; i += interval, j++)
        {
            valueList.Add(inputList[i].getOption(option));
        }
        ShowGraph_Helper(valueList, option);
    }

    //apparently it is much more efficient to adjust existing objects than spawning new gameobjects
    private void ShowGraph_Helper (List<float> valueList, GraphOptions option)
    { 
        //for runtime testing
        //System.Diagnostics.Stopwatch stopWatch = new Stopwatch();
        //stopWatch.Start();

        //destroys all previous objects;

        //.Debug.Log("Count: " + inputList.Count);

        //destroying objects seem a bit costly, should optimize later
        for (int i = 0; i < gameObjectList.Count; i++)
        {
            Destroy(gameObjectList[i]);
        }
        gameObjectList.Clear();

        if (valueList.Count == 0) return;

        //set the new graph
        float graphHeight = graphContainer.rect.height;
        float graphWidth = graphContainer.rect.width;
        float yMax = 0f;
        float yMin = float.PositiveInfinity;
        for (int i = 0; i < valueList.Count; i++)
        {
            yMax = Mathf.Max(yMax, valueList[i]);
            yMin = Mathf.Min(yMin, valueList[i]);
        }
        float range = yMax - yMin;
        if (Math.Abs(range) < EPSILON) { range = yMax; } //accounts for the case of a horizontal line
        yMax += range * 0.1f;
        yMin -= range * 0.1f;
        range = yMax - yMin;
        float timestep = Time.fixedDeltaTime * 1000; //later can be changed
        float xMax = (valueList.Count - 1)* timestep; //seconds for display
        float xMin = 0f;
        float domain = xMax - xMin;

        GameObject prev = null;
        for (int i = 0; i < valueList.Count; i++)
        {
            float xpos = i * 1f / valueList.Count * graphWidth;
            //UnityEngine.Debug.Log(xpos);
            float ypos = (valueList[i] - yMin) / range * graphHeight;
            GameObject curr = CreateCircle(new Vector2(xpos, ypos));
            gameObjectList.Add(curr);
            if (prev != null) gameObjectList.Add(CreateDotConnection(prev.GetComponent<RectTransform>().anchoredPosition, curr.GetComponent<RectTransform>().anchoredPosition));
            prev = curr;
        }

        //X-axis labels:
        int separatorCountX = 5;
        for (int i = 0; i <= separatorCountX; i++)
        {
            RectTransform labelX = Instantiate(labelTemplateX);
            labelX.SetParent(graphContainer);
            labelX.gameObject.SetActive(true);
            float normalizedValue = i * 1f / separatorCountX;
            labelX.anchoredPosition = new Vector2(normalizedValue*graphWidth, -20f);
            Text text = labelX.GetComponent<Text>();
            //this rounding this is still kind of sketchy
            text.text = Mathf.RoundToInt(xMin + normalizedValue * domain).ToString();
            text.alignment = TextAnchor.MiddleCenter;
            gameObjectList.Add(labelX.gameObject);

            RectTransform dashY = Instantiate(dashTemplateY);
            dashY.SetParent(graphContainer, false);
            dashY.gameObject.SetActive(true);
            dashY.anchoredPosition = new Vector2(normalizedValue * graphWidth, 0f);
            dashY.sizeDelta = new Vector2(2f, graphHeight);
            gameObjectList.Add(dashY.gameObject);
        }

        //Y-axis labels:
        int separatorCountY = 10;
        for (int i = 0; i <= separatorCountY; i++)
        {
            RectTransform labelY = Instantiate(labelTemplateY);
            labelY.SetParent(graphContainer);
            labelY.gameObject.SetActive(true);
            float normalizedValue = i * 1f / separatorCountY;
            labelY.anchoredPosition = new Vector2(-20f, normalizedValue*graphHeight);
            Text text = labelY.GetComponent<Text>();
            //this rounding thing is kind of sketchy.
            text.text = Mathf.RoundToInt(yMin + normalizedValue * range).ToString();
            text.alignment = TextAnchor.MiddleCenter;
            gameObjectList.Add(labelY.gameObject);

            //create horizontal
            RectTransform dashX = Instantiate(dashTemplateX);
            dashX.SetParent(graphContainer, false);
            dashX.gameObject.SetActive(true);
            dashX.anchoredPosition = new Vector2(0f, normalizedValue * graphHeight);
            dashX.sizeDelta = new Vector2(graphWidth, 2f);
            gameObjectList.Add(dashX.gameObject);
        }

        //Code
        //stopWatch.Stop();
        //// Get the elapsed time as a TimeSpan value.
        //TimeSpan ts = stopWatch.Elapsed;
        //UnityEngine.Debug.Log(ts.TotalMilliseconds);
    }

    private GameObject CreateDotConnection (Vector2 posA, Vector2 posB)
    {
        GameObject obj = new GameObject("dot_connection", typeof(Image));
        obj.transform.SetParent(graphContainer, false);
        obj.GetComponent<Image>().color = new Color(1, 1, 1);
        RectTransform rectTransform = obj.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2 (0f, 0f);
        rectTransform.anchorMax = new Vector2(0f, 0f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        Vector2 connectionPos = posA + (posB - posA) / 2;
        float dis = Vector2.Distance(posA, posB);
        rectTransform.sizeDelta = new Vector2(dis, 4f);
        rectTransform.anchoredPosition = connectionPos;
        rectTransform.localEulerAngles = new Vector3(0, 0, Util.GetAngleFromVectorFloat(posB - posA));
        return obj;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
