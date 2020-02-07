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

    private static int numDataPoints = 300;
    private static int separatorCountX = 5;
    private static int separatorCountY = 10;
    private float graphHeight;
    private float graphWidth;
    private float range;
    private float domain;
    private float yMax = float.NegativeInfinity;
    private float yMin = float.PositiveInfinity;
    private float xMax = 0f;
    private float xMin = 0f;

    private AudioSource buttonAudio;

    private GraphButtons buttonAccelerationX;
    private GraphButtons buttonAccelerationY;
    private GraphButtons buttonVelocityX;
    private GraphButtons buttonVelocityY;
    private GraphButtons buttonPositionX;
    private GraphButtons buttonPositionY;

    private RectTransform[][] dataPoints = new RectTransform[][]
    {
        new RectTransform [numDataPoints],
        new RectTransform [numDataPoints],
        new RectTransform [numDataPoints],
        new RectTransform [numDataPoints],
        new RectTransform [numDataPoints],
        new RectTransform [numDataPoints],
    };

    private RectTransform[][] dot_connections = new RectTransform[][]
    {
        new RectTransform [numDataPoints - 1],
        new RectTransform [numDataPoints - 1],
        new RectTransform [numDataPoints - 1],
        new RectTransform [numDataPoints - 1],
        new RectTransform [numDataPoints - 1],
        new RectTransform [numDataPoints - 1],
    };

    private RectTransform[] separatorsX = new RectTransform[separatorCountX + 1];
    private RectTransform[] separatorsY = new RectTransform[separatorCountY + 1];
    private Text[] separatorsXText = new Text[separatorCountX + 1];
    private Text[] separatorsYText = new Text[separatorCountY + 1];

    private IDictionary<GraphOptions, int> lookup = new Dictionary<GraphOptions, int> ();
    private GraphOptions[] lookup2 = new GraphOptions[6] { GraphOptions.accelerationX, GraphOptions.accelerationY, GraphOptions.velocityX, GraphOptions.velocityY, GraphOptions.positionX, GraphOptions.positionY };
    private Color[] graphColors = Util.graphColors;

    private void Awake()
    {
        ReplayControl.graphWindow = this.gameObject.GetComponent<RectTransform>();
        ReplayControl.gc = this;
        graphContainer = this.gameObject.transform.Find("GraphContainer").GetComponent<RectTransform>();
        labelTemplateX = graphContainer.Find("LabelTemplateX").GetComponent<RectTransform>();
        labelTemplateY = graphContainer.Find("LabelTemplateY").GetComponent<RectTransform>();
        dashTemplateX = graphContainer.Find("DashTemplateX").GetComponent<RectTransform>();
        dashTemplateY = graphContainer.Find("DashTemplateY").GetComponent<RectTransform>();
        buttonAccelerationX = GameObject.Find("AccelerationX").GetComponent<GraphButtons>();
        buttonAccelerationY = GameObject.Find("AccelerationY").GetComponent<GraphButtons>();
        buttonVelocityX = GameObject.Find("VelocityX").GetComponent<GraphButtons>();
        buttonVelocityY = GameObject.Find("VelocityY").GetComponent<GraphButtons>();
        buttonPositionX = GameObject.Find("DisplacementX").GetComponent<GraphButtons>();
        buttonPositionY = GameObject.Find("DisplacementY").GetComponent<GraphButtons>();
        buttonAudio = GameObject.Find("ButtonClickAudioSource").GetComponent<AudioSource>();
        graphHeight = graphContainer.rect.height;
        graphWidth = graphContainer.rect.width;

        lookup.Add(GraphOptions.accelerationX, 0);
        lookup.Add(GraphOptions.accelerationY, 1);
        lookup.Add(GraphOptions.velocityX, 2);
        lookup.Add(GraphOptions.velocityY, 3);
        lookup.Add(GraphOptions.positionX, 4);
        lookup.Add(GraphOptions.positionY, 5);

        //initialize data points
        for (int j = 0; j < 6; j++)
        {
            for (int i = 0; i < numDataPoints; i++)
            {
                GameObject obj = new GameObject("dot", typeof(Image));
                obj.transform.SetParent(graphContainer, false);
                Image image = obj.GetComponent<Image>();
                image.sprite = circleSprite;
                image.color = graphColors[j];
                RectTransform rectTransform = obj.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(2f, 2f);
                rectTransform.anchorMax = new Vector2(0f, 0f);
                rectTransform.anchorMin = new Vector2(0f, 0f);
                rectTransform.pivot = new Vector2(0.5f, 0.5f);
                obj.SetActive(false);
                dataPoints[j][i] = rectTransform;
            }
            for (int i = 0; i < numDataPoints - 1; i++)
            {
                GameObject obj = new GameObject("dot_connection", typeof(Image));
                obj.transform.SetParent(graphContainer, false);
                obj.GetComponent<Image>().color = graphColors[j];
                RectTransform rectTransform = obj.GetComponent<RectTransform>();
                rectTransform.anchorMin = new Vector2(0f, 0f);
                rectTransform.anchorMax = new Vector2(0f, 0f);
                rectTransform.pivot = new Vector2(0.5f, 0.5f);
                obj.SetActive(false);
                dot_connections[j][i] = rectTransform;
            }
        }
        
        for (int i = 0; i <= separatorCountX; i++)
        {
            float normalizedValue = i * 1f / separatorCountX;

            RectTransform labelX = Instantiate(labelTemplateX);
            labelX.SetParent(graphContainer);
            labelX.gameObject.SetActive(true);
            labelX.anchoredPosition = new Vector2(normalizedValue * graphWidth, -20f);
            Text text = labelX.GetComponent<Text>();
            text.alignment = TextAnchor.MiddleCenter;
            text.font = Util.Caladea_Bold;
            separatorsXText[i] = text;

            RectTransform dashY = Instantiate(dashTemplateY);
            dashY.SetParent(graphContainer, false);
            dashY.gameObject.SetActive(true);
            dashY.anchoredPosition = new Vector2(normalizedValue * graphWidth, 0f);
            dashY.sizeDelta = new Vector2(2f, graphHeight);
            separatorsX[i] = dashY;
        }
        for (int i = 0; i <= separatorCountY; i++)
        {
            float normalizedValue = i * 1f / separatorCountY;

            RectTransform labelY = Instantiate(labelTemplateY);
            labelY.SetParent(graphContainer);
            labelY.gameObject.SetActive(true);
            labelY.anchoredPosition = new Vector2(-20f, normalizedValue * graphHeight);
            Text text = labelY.GetComponent<Text>();
            text.alignment = TextAnchor.MiddleRight;
            text.font = Util.Caladea_Bold;
            separatorsYText[i] = text;

            RectTransform dashX = Instantiate(dashTemplateX);
            dashX.SetParent(graphContainer, false);
            dashX.gameObject.SetActive(true);
            dashX.anchoredPosition = new Vector2(0f, normalizedValue * graphHeight);
            dashX.sizeDelta = new Vector2(graphWidth, 2f);
            separatorsY[i] = dashX;
        }
        
    }

    ////makes a circle
    //private GameObject CreateCircle (Vector2 anchorPosition)
    //{
    //    GameObject obj = new GameObject("dot", typeof(Image));
    //    obj.transform.SetParent(graphContainer, false);
    //    obj.GetComponent<Image>().sprite = circleSprite;
    //    RectTransform rectTransform = obj.GetComponent<RectTransform>();
    //    rectTransform.anchoredPosition = anchorPosition;
    //    rectTransform.sizeDelta = new Vector2(4f, 4f);
    //    rectTransform.anchorMax = new Vector2(0f, 0f);
    //    rectTransform.anchorMin = new Vector2(0f, 0f);
    //    rectTransform.pivot = new Vector2(0.5f, 0.5f);
    //    return obj;
    //}

    public void ShowGraph (List<PointInTime> inputList, HashSet <GraphOptions> options)
    {
        //for runtime testing
        //System.Diagnostics.Stopwatch stopWatch = new Stopwatch();
        //stopWatch.Start();

        //destroys all previous objects;

        //UnityEngine.Debug.Log("Count: " + inputList.Count);

        yMax = float.NegativeInfinity;
        yMin = float.PositiveInfinity;

        List<float>[] lists = new List<float>[6];
        for (int i = 0; i < 6; i++) lists[i] = new List<float>();

        foreach (GraphOptions each in options)
        {
            int idx;
            lookup.TryGetValue(each, out idx);
            for (int i = Mathf.Max(0, inputList.Count - numDataPoints); i < inputList.Count; i++)
            {
                float temp = inputList[i].getOption(each);
                lists[idx].Add(temp);
                yMax = Mathf.Max(temp, yMax);
                yMin = Mathf.Min(temp, yMin);
            }
        }

        this.range = yMax - yMin;
        if (Math.Abs(range) < Util.EPSILON_SMALL) { this.range = yMax; } //accounts for the case of a horizontal line
        if (Math.Abs(range) < Util.EPSILON_SMALL) { this.range = 10f; } //if yMax happens to be zero
        yMax += this.range * 0.1f;
        yMin -= this.range * 0.1f;
        this.range = yMax - yMin;

        float timestep = Time.fixedDeltaTime; //later can be changed
        xMax = (inputList.Count - 1) * timestep; //seconds for display
        xMin = Mathf.Max(0f, inputList.Count - numDataPoints) * timestep;
        this.domain = xMax - xMin;

        for (int i = 0; i < 6; i++) ShowGraph_Helper(lists[i], lookup2[i]);

        AdjustTexts();

        ///Code
        //stopWatch.Stop();
        //// Get the elapsed time as a TimeSpan value.
        //TimeSpan ts = stopWatch.Elapsed;
        //UnityEngine.Debug.Log(ts.TotalMilliseconds);
    }

    //overload
    public void ShowGraph (List<PointInTime> inputList, HashSet <GraphOptions> options, int maxIndex)
    {
        //System.Diagnostics.Stopwatch stopWatch = new Stopwatch();
        //stopWatch.Start();

        yMax = float.NegativeInfinity;
        yMin = float.PositiveInfinity;

        List<float>[] lists = new List<float>[6];
        for (int i = 0; i < 6; i++) lists[i] = new List<float>();

        foreach (GraphOptions each in options)
        {
            int idx;
            lookup.TryGetValue(each, out idx);
            for (int i = Mathf.Max(0, maxIndex + 1 - numDataPoints); i <= maxIndex; i++)
            {
                float temp = inputList[i].getOption(each);
                lists[idx].Add(temp);
                yMax = Mathf.Max(temp, yMax);
                yMin = Mathf.Min(temp, yMin);
            }
        }

        this.range = yMax - yMin;
        if (Math.Abs(range) < Util.EPSILON_SMALL) { this.range = yMax; } //accounts for the case of a horizontal line
        if (Math.Abs(range) < Util.EPSILON_SMALL) { this.range = 10f; } //if yMax happens to be zero
        yMax += this.range * 0.1f;
        yMin -= this.range * 0.1f;
        this.range = yMax - yMin;

        float timestep = Time.fixedDeltaTime; //later can be changed
        xMax = maxIndex * timestep; //seconds for display
        xMin = Mathf.Max(0f, maxIndex - numDataPoints + 1) * timestep;
        this.domain = xMax - xMin;

        for (int i = 0; i < 6; i++) ShowGraph_Helper(lists[i], lookup2[i]);
        AdjustTexts();

        //Code
        //stopWatch.Stop();
        //// Get the elapsed time as a TimeSpan value.
        //TimeSpan ts = stopWatch.Elapsed;
        //UnityEngine.Debug.Log(ts.TotalMilliseconds);
    }

    private void AdjustTexts()
    {
        for(int i = 0; i <= separatorCountX; i++)
        {
            float normalizedValue = i * 1f / separatorCountX;
            float value = xMin + normalizedValue * domain;
            separatorsXText[i].text = (domain < 5f ? Mathf.Round(value * 10f) / 10f : Mathf.RoundToInt(value)).ToString();
        }
        for (int i = 0; i <= separatorCountY; i++)
        {
            float normalizedValue = i * 1f / separatorCountY;
            float value = yMin + normalizedValue * range;
            separatorsYText[i].text = (range < 100f ? Mathf.Round(value * 100f) / 100f : Mathf.RoundToInt(value)).ToString();
        }
    }

    private void ShowGraph_Helper (List<float> valueList, GraphOptions option)
    {
        int idx;
        lookup.TryGetValue(option, out idx);

        RectTransform prev = null;
        for (int i = 0; i < numDataPoints; i++)
        {
            if (i < valueList.Count)
            {
                float xpos = i * 1f / valueList.Count * graphWidth;
                float ypos = (valueList[i] - this.yMin) / range * graphHeight;
                RectTransform curr = dataPoints[idx][i];
                AdjustCircle(curr, new Vector2(xpos, ypos));
                if (prev != null) AdjustDotConnection(dot_connections[idx][i - 1], prev.anchoredPosition, curr.anchoredPosition);
                prev = curr;
            }
            else
            {
                dataPoints[idx][i].gameObject.SetActive(false);
                dot_connections[idx][Mathf.Min(numDataPoints - 2, i)].gameObject.SetActive(false);
            }
        }
    }

    private void AdjustCircle(RectTransform circle, Vector2 anchorPosition)
    {
        circle.gameObject.SetActive(true);
        circle.anchoredPosition = anchorPosition;
    }

    private void AdjustDotConnection (RectTransform dot_connection, Vector2 posA, Vector2 posB)
    {
        dot_connection.gameObject.SetActive(true);
        Vector2 connectionPos = (posB + posA) / 2;
        float dis = Vector2.Distance(posA, posB);
        dot_connection.sizeDelta = new Vector2(dis, 4f);
        dot_connection.anchoredPosition = connectionPos;
        dot_connection.localEulerAngles = new Vector3(0, 0, Util.GetAngleFromVectorFloat(posB - posA));
    }

    public void OnClickAccelerationX()
    {
        if (PropertiesControlAreaScript.focusedObject == null) return;
        ReplayControl rc = PropertiesControlAreaScript.focusedObject.GetComponent<ReplayControl>();
        if (rc == null) return;
        buttonAudio.Play();
        buttonAccelerationX.SetSelected(!buttonAccelerationX.selected);
        if (buttonAccelerationX.selected) rc.graphOptions.Add(GraphOptions.accelerationX);
        else rc.graphOptions.Remove(GraphOptions.accelerationX);
    }

    public void OnClickAccelerationY()
    {
        if (PropertiesControlAreaScript.focusedObject == null) return;
        ReplayControl rc = PropertiesControlAreaScript.focusedObject.GetComponent<ReplayControl>();
        if (rc == null) return;
        buttonAudio.Play();
        buttonAccelerationY.SetSelected(!buttonAccelerationY.selected);
        if (buttonAccelerationY.selected) rc.graphOptions.Add(GraphOptions.accelerationY);
        else rc.graphOptions.Remove(GraphOptions.accelerationY);
    }

    public void OnClickVelocityX()
    {
        if (PropertiesControlAreaScript.focusedObject == null) return;
        ReplayControl rc = PropertiesControlAreaScript.focusedObject.GetComponent<ReplayControl>();
        if (rc == null) return;
        buttonAudio.Play();
        buttonVelocityX.SetSelected(!buttonVelocityX.selected);
        if (buttonVelocityX.selected) rc.graphOptions.Add(GraphOptions.velocityX);
        else rc.graphOptions.Remove(GraphOptions.velocityX);
    }

    public void OnClickVelocityY()
    {
        if (PropertiesControlAreaScript.focusedObject == null) return;
        ReplayControl rc = PropertiesControlAreaScript.focusedObject.GetComponent<ReplayControl>();
        if (rc == null) return;
        buttonAudio.Play();
        buttonVelocityY.SetSelected(!buttonVelocityY.selected);
        if (buttonVelocityY.selected) rc.graphOptions.Add(GraphOptions.velocityY);
        else rc.graphOptions.Remove(GraphOptions.velocityY);
    }

    public void OnClickDisplacementX()
    {
        if (PropertiesControlAreaScript.focusedObject == null) return;
        ReplayControl rc = PropertiesControlAreaScript.focusedObject.GetComponent<ReplayControl>();
        if (rc == null) return;
        buttonAudio.Play();
        buttonPositionX.SetSelected(!buttonPositionX.selected);
        if (buttonPositionX.selected) rc.graphOptions.Add(GraphOptions.positionX);
        else rc.graphOptions.Remove(GraphOptions.positionX);
    }

    public void OnClickDisplacementY()
    {
        if (PropertiesControlAreaScript.focusedObject == null) return;
        ReplayControl rc = PropertiesControlAreaScript.focusedObject.GetComponent<ReplayControl>();
        if (rc == null) return;
        buttonAudio.Play();
        buttonPositionY.SetSelected(!buttonPositionY.selected);
        if (buttonPositionY.selected) rc.graphOptions.Add(GraphOptions.positionY);
        else rc.graphOptions.Remove(GraphOptions.positionY);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (PropertiesControlAreaScript.focusedObject == null) return;
        ReplayControl rc = PropertiesControlAreaScript.focusedObject.GetComponent<ReplayControl>();
        if (rc == null) return;
        buttonAccelerationX.SetSelected(rc.graphOptions.Contains(GraphOptions.accelerationX));
        buttonAccelerationY.SetSelected(rc.graphOptions.Contains(GraphOptions.accelerationY));
        buttonVelocityX.SetSelected(rc.graphOptions.Contains(GraphOptions.velocityX));
        buttonVelocityY.SetSelected(rc.graphOptions.Contains(GraphOptions.velocityY));
        buttonPositionX.SetSelected(rc.graphOptions.Contains(GraphOptions.positionX));
        buttonPositionY.SetSelected(rc.graphOptions.Contains(GraphOptions.positionY));
    }
}