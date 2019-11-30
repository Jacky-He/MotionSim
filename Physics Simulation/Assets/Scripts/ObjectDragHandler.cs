using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectDragHandler : MonoBehaviour, IDragHandler, IEndDragHandler
{

    private Vector3 originalPosition;

    public int index;
    public float spacing;

    public void OnDrag(PointerEventData eventData)
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            transform.position = Input.touches[0].position;
        }
        else
        {
            transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.localPosition = new Vector3(0, -((170+spacing)*index+85), 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
