using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    private Vector3 originalPosition;

    public int index;
    public float spacing;

    private Transform parentToReturnTo = null;

    public GameObject spritePreFab;

    public void OnBeginDrag (PointerEventData eventData)
    {
        this.parentToReturnTo = this.transform.parent;
        this.transform.SetParent(this.transform.parent.parent.parent.parent); //the canvass

        //disable the blocking of the raycast for the object so that the canvass can receive the signal
        this.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        this.transform.SetParent(this.parentToReturnTo);
        transform.localPosition = new Vector3(0, -((170 + spacing) * index + 85), 0);
        
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
