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

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (ReplayControl.focusedObject != null && ReplayControl.focusedObject.GetComponent<ReplayControl>() != null && graph)
        {
            if (!grapharea.activeSelf)
            {
                graph = false;
                OnClickGraphTab();
            }
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
}
