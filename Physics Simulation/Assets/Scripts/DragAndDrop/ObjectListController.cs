using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ObjectListController : MonoBehaviour
{
    [SerializeField]
    private GameObject circleTemplate;
    [SerializeField]
    private GameObject fixedRectTemplate;
    [SerializeField]
    private GameObject scrollContentView;

    private List<GameObject> templateList = new List <GameObject> ();

    //stores the objects that are displayed which are instantiated from templateList every time populate is called
    private List<GameObject> existingRefs = new List<GameObject>();

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
            GameObject obj = Instantiate(templateList [i]) as GameObject;
            obj.SetActive(true);
            obj.GetComponent<ObjectDragHandler>().index = i;
            obj.GetComponent<ObjectDragHandler>().spacing = scrollContentView.GetComponent<VerticalLayoutGroup>().spacing;
            obj.GetComponent<Image>().color = Random.ColorHSV();
            obj.transform.SetParent(templateList[i].transform.parent, false);
            existingRefs.Add(obj);
        }
    }

    private void PopulateList ()
    {
        //Adds the objects into the scrollview
        templateList.Add(circleTemplate);
        templateList.Add(fixedRectTemplate);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        PopulateList();
        PopulateView();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
