using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ObjectDropHandler : MonoBehaviour, IDropHandler
{    
    public void OnDrop (PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;
        Vector3 pos = eventData.position;
        //check if pos is inside of scrollViewRect
        if (Util.OnCanvas(pos))
        {
            //this is the object being dropped
            //instantiate a new object in the canvass
            GameObject ui = eventData.pointerDrag;
            GameObject reference = ui.GetComponent<ObjectDragHandler>().spritePreFab;
            GameObject newObj = Instantiate(reference) as GameObject;
            newObj.SetActive(true);
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
            worldPos.z = 0;
            newObj.transform.position = worldPos;

            //Check for spring
            SpringControl spring = newObj.GetComponent<SpringControl>();
            ForceControl force = newObj.GetComponent<ForceControl>();
            VelocityControl velocity = newObj.GetComponent<VelocityControl>();
            if (spring != null) //if this is a spring
            {
                spring.attachPoint1 = worldPos + new Vector3(0, SpringControl.defaultHeight / 2f, 0);
                spring.attachPoint2 = worldPos - new Vector3(0, SpringControl.defaultHeight / 2f, 0);
            }
            if (force != null) //if this is a force arrow
            {
                force.attachPoint1 = worldPos + new Vector3(0, ForceControl.defaultHeight / 2f, 0);
                force.attachPoint2 = worldPos - new Vector3(0, ForceControl.defaultHeight / 2f, 0);
            }
            if (velocity != null)
            {
                velocity.attachPoint1 = worldPos + new Vector3(0, VelocityControl.defaultHeight / 2f, 0);
                velocity.attachPoint2 = worldPos - new Vector3(0, VelocityControl.defaultHeight / 2f, 0);
            }
        }
        //otherwise do nothing;
    }
}
