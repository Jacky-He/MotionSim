using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SliderControl : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    public static float percentage = 0f;

    private Slider slider;

    public void OnValueChange (float newVal)
    {
        percentage = newVal / (slider.maxValue - slider.minValue);
        //Debug.Log(percentage);
    }

    public void OnBeginDrag (PointerEventData eventData)
    {
        Util.sliderSelected = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Util.sliderSelected = false;
    }


    // Start is called before the first frame update
    void Start()
    {
        slider = this.transform.GetComponent<Slider>();
        slider.value = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
