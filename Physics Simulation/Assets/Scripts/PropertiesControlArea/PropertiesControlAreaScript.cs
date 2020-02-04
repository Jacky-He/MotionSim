using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using v2 = UnityEngine.Vector2;
using v3 = UnityEngine.Vector3;
using go = UnityEngine.GameObject;
using rtm = UnityEngine.RectTransform;
using tm = UnityEngine.Transform;

public class PropertiesControlAreaScript : MonoBehaviour
{
    public static go focusedObject
    {
        get { return PropertiesEditable.focusedObject; }
        set { PropertiesEditable.focusedObject = value; }
    }

    private go grapharea;
    private go propertiesarea;
    private go graphTab;
    private go propertiesTab;

    private GraphControl gc;

    bool graph = false;

    
    // Update is called once per frame
    void Update()
    {
        if (focusedObject != null && focusedObject.GetComponent<ReplayControl>() != null && graph)
        {
            if (!grapharea.activeSelf)
            {
                graph = false;
                OnClickGraphTab();
            }
        }
        if (focusedObject == null)
        {
            if (grapharea.activeSelf && graph) grapharea.SetActive(false);
        }
        else
        {
            Name name = focusedObject.GetComponent<Name>();
            if (name.objectname == "Spring" || name.objectname == "Force" || name.objectname == "Velocity")
            {
                if (grapharea.activeSelf && graph) grapharea.SetActive(false);
            }
        }

        //animation
        if (propertiesControlAreaShowing)
        {
            v3 oldpos = propertiesControlAreaRTM.anchoredPosition;
            if (oldpos.x <= propertiesControlAreaOutXpos) propertiesControlAreaRTM.anchoredPosition = new v2(propertiesControlAreaOutXpos, oldpos.y);
            propertiesControlAreaRTM.anchoredPosition = new v2(Mathf.Max(oldpos.x - propertiesControlAreaAnimationShift / propertiesControlAreaAnimationTime * Time.deltaTime, propertiesControlAreaOutXpos), oldpos.y);
        }
        else
        {
            v3 oldpos = propertiesControlAreaRTM.anchoredPosition;
            if (oldpos.x <= propertiesControlAreaInXpos) propertiesControlAreaRTM.anchoredPosition = new v2(propertiesControlAreaInXpos, oldpos.y);
            propertiesControlAreaRTM.anchoredPosition = new v2(Mathf.Min(oldpos.x + propertiesControlAreaAnimationShift / propertiesControlAreaAnimationTime * Time.deltaTime, propertiesControlAreaInXpos), oldpos.y);
        }
    }

    private void Awake()
    {
        graphTab = this.gameObject.transform.Find("GraphTab").gameObject;
        propertiesTab = this.gameObject.transform.Find("PropertiesTab").gameObject;

        grapharea = this.gameObject.GetComponent<rtm>().Find("GraphArea").Find("GraphAreaContainer").gameObject;
        propertiesarea = this.gameObject.GetComponent<rtm>().Find("PropertiesArea").gameObject;
        gc = go.Find("GraphWindow").GetComponent<GraphControl>();

        OnClickGraphTab();

        //animation
        canvas = go.Find("Canvas");
        propertiesControlArea = go.Find("PropertiesControlArea");
        propertiesControlAreaRTM = propertiesControlArea.GetComponent<rtm>();
    }

    public void OnClickGraphTab()
    {
        if (graph) return;
        graph = true;
        graphTab.transform.SetAsLastSibling();
        graphTab.GetComponent<Image>().color = Util.GraphTabOnColor;
        propertiesTab.GetComponent<Image>().color = Util.PropertiesTabOffColor;

        grapharea.SetActive(true);
        propertiesarea.SetActive(false);
        if (focusedObject == null) { grapharea.SetActive(false); return; }
        ReplayControl rc = focusedObject.GetComponent<ReplayControl>();
        if (rc == null) grapharea.SetActive(false);
    }

    public void OnClickPropertiesTab()
    {
        if (!graph) return;
        graph = false;
        propertiesTab.transform.SetAsLastSibling();
        graphTab.GetComponent<Image>().color = Util.GraphTabOffColor;
        propertiesTab.GetComponent<Image>().color = Util.PropertiesTabOnColor;

        grapharea.SetActive(false);
        propertiesarea.SetActive(true);
    }


    //animation
    private go canvas;
    private rtm propertiesControlAreaRTM;
    private go propertiesControlArea;

    //private static bool propertiesControlAreaShown = true;
    public static bool propertiesControlAreaShowing = true;

    //private static float propertiesControlAreaAnimationTime = 1f; //seconds
    public static float propertiesControlAreaAnimationTime = 0.2f; //seconds

    //private static float propertiesControlAreaAnimationShift = 1f;
    public static float propertiesControlAreaAnimationShift = 1f;

    public static float propertiesControlAreaOutXpos;
    public static float propertiesControlAreaInXpos;

    // Use this for initialization
    void Start()
    {
        ApplySafeArea();
    }

    private void ApplySafeArea()
    {
        Rect safeArea = Screen.safeArea;

        v3 safeAreaPos = safeArea.position / canvas.transform.localScale.x;

        propertiesControlAreaOutXpos = Application.isMobilePlatform ? -safeAreaPos.x : 0f;

        propertiesControlAreaInXpos = 651f;

        propertiesControlAreaAnimationShift = Mathf.Abs(propertiesControlAreaOutXpos - propertiesControlAreaInXpos);
    }
}
