using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ObjectDropHandler : MonoBehaviour, IDropHandler
{


    [SerializeField]
    private GameObject scrollView;

    public void OnDrop (PointerEventData eventData)
    {
        RectTransform scrollViewRect = scrollView.transform as RectTransform;
        Vector3 pos = eventData.position;
        //check if pos is inside of scrollViewRect
        if (!RectTransformUtility.RectangleContainsScreenPoint(scrollViewRect, pos))
        {
            //this is the object being dropped
            //instantiate a new object in the canvass
            GameObject ui = eventData.pointerDrag;
            GameObject reference = ui.GetComponent<ObjectDragHandler>().spritePreFab;
            GameObject newObj = Instantiate(reference) as GameObject;
            newObj.SetActive(true);
            newObj.transform.position = eventData.position;
            
            //GameObject obj = Instantiate(eventData.pointerDrag) as GameObject;
            //obj.SetActive(true);
            //obj.GetComponent<CanvasGroup>().blocksRaycasts = true;
            //obj.transform.SetParent(eventData.pointerDrag.transform.parent, false);
            //obj.transform.position = eventData.position;
            //implement drop code
        }
        //otherwise do nothing;
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
