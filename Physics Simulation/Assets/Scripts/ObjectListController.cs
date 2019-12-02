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
    private GameObject scrollContentView;

    private List<GameObject> templateList = new List <GameObject> ();

    //stores the objects that are displayed which are instantiated from templateList every time populate is called
    private List<GameObject> existingRefs = new List<GameObject>();

    ////handling the dropping of the objects
    //public void OnDrop(PointerEventData eventData)
    //{
    //    RectTransform scrollViewRect = transform as RectTransform;
    //    Vector3 pos;
    //    pos = eventData.position;
    //    //checks if pos is inside of scrollViewRect
    //    if (!RectTransformUtility.RectangleContainsScreenPoint(scrollViewRect, pos))
    //    {
    //        //implement drop code
    //    }
    //    //otherwise, do nothing;
    //}

    //Populate the list
    private void Populate ()
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

        //adds the objects into the scrollview
        for (int x = 0; x < 20; x++)
        {
            templateList.Add(circleTemplate);
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

    // Start is called before the first frame update
    void Start()
    {
        Populate();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
