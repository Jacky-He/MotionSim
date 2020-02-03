using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ObjectListController : MonoBehaviour
{
    [SerializeField] private GameObject springTemplate;
    [SerializeField] private GameObject circleTemplate;
    [SerializeField] private GameObject fixedRectTemplate;
    [SerializeField] private GameObject moveableRectTemplate;
    [SerializeField] private GameObject scrollContentView;
    [SerializeField] private GameObject forceTemplate;
    [SerializeField] private GameObject velocityTemplate;

    private List<GameObject> templateList = new List <GameObject> ();
    private List<GameObject> labels = new List<GameObject>();

    //stores the objects that are displayed which are instantiated from templateList every time populate is called
    private List<GameObject> existingRefs = new List<GameObject>();

    private string [] labelNames = new string[6] { "Circle", "Fixed Rect", "Moveable Rect", "Spring", "Force", "Initial Velocity" };

    //Populate the list
    private void PopulateView ()
    {
        //if populate is called again, make sure to destroy the previous objects so that new objects can be instantiated.
        if (existingRefs.Count > 0)
        {
            foreach (GameObject obj in existingRefs)
            {
                Destroy(obj);
            }
            existingRefs.Clear();
        }

        for (int i = 0; i < templateList.Count; i++)
        {
            GameObject obj = templateList[i];
            obj.SetActive(true);
            obj.transform.SetParent(scrollContentView.transform, false);
            existingRefs.Add(obj);

            GameObject obj2 = labels[i];
            obj2.SetActive(true);
            obj2.transform.SetParent(scrollContentView.transform, false);
            existingRefs.Add(obj2);
        }
    }

    private void PopulateList ()
    {
        //Adds the objects into the scrollview
        templateList.Add(Instantiate(circleTemplate) as GameObject);
        templateList.Add(Instantiate(fixedRectTemplate) as GameObject);
        templateList.Add(Instantiate(moveableRectTemplate) as GameObject);
        templateList.Add(Instantiate(springTemplate) as GameObject);
        templateList.Add(Instantiate(forceTemplate) as GameObject);
        templateList.Add(Instantiate(velocityTemplate) as GameObject);
        for (int i = 0; i < 6; i++)
        {
            GameObject label = new GameObject("label", typeof(Text));
            label.GetComponent<RectTransform>().sizeDelta = new Vector2(250, 40);
            Text t = label.GetComponent<Text>();
            t.font = Util.Caladea_Bold;
            t.color = Color.white;
            t.fontSize = 30;
            t.alignment = TextAnchor.MiddleCenter;
            t.text = labelNames[i];
            labels.Add(label);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        PopulateList();
        PopulateView();
    }

    private void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
