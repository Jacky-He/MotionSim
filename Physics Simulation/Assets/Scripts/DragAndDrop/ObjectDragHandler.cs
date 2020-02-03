using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    private Vector3 originalPosition;

    private Transform parentToReturnTo = null;

    public GameObject spritePreFab;

    public GameObject selfCopy;

    public void OnBeginDrag (PointerEventData eventData)
    {
        //Initiate a copy of this object
        selfCopy = Instantiate(this.gameObject) as GameObject;
        selfCopy.GetComponent<ObjectDragHandler>().enabled = false;
        selfCopy.transform.SetParent(this.transform.parent.parent.parent.parent); //the canvass

        //disable the blocking of the raycast for the object so that the canvass can receive the signal
        this.GetComponent<CanvasGroup>().blocksRaycasts = false;
        selfCopy.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        selfCopy.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Destroy(selfCopy);
        //turns the thing back on
        this.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        originalPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
