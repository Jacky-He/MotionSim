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

    private GraphControl gc;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Awake()
    {
        grapharea = this.gameObject.GetComponent<rtm>().Find("GraphArea").gameObject;
        propertiesarea = this.gameObject.GetComponent<rtm>().Find("PropertiesArea").gameObject;
        gc = go.Find("GraphWindow").GetComponent<GraphControl>();
    }

    public void OnClickGraphTab()
    {
        grapharea.SetActive(true);
        propertiesarea.SetActive(false);
        ReplayControl rc = focusedObject.GetComponent<ReplayControl>();
        if (rc == null) grapharea.SetActive(false);
    }

    public void OnClickPropertiesTab()
    {
        grapharea.SetActive(false);
        propertiesarea.SetActive(true);
    }
}
