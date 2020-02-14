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
            ObjectDragHandler draghandler = ui.GetComponent<ObjectDragHandler>();
            if (draghandler == null) return;
            GameObject reference = draghandler.spritePreFab;
            GameObject newObj = Instantiate(reference) as GameObject;
            newObj.SetActive(true);
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
            worldPos.z = 0;
            newObj.transform.position = worldPos;

            //Check for spring
            SpringControl spring = newObj.GetComponent<SpringControl>();
            ForceControl force = newObj.GetComponent<ForceControl>();
            VelocityControl velocity = newObj.GetComponent<VelocityControl>();
            PropertiesEditable edit = newObj.GetComponent<PropertiesEditable>();
            ReplayControl replay = newObj.GetComponent<ReplayControl>();
            Name name = newObj.GetComponent<Name>();
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
            if (replay != null) ReplayControl.focusedObject = newObj;
            if (edit != null) PropertiesEditable.focusedObject = newObj;
            //if (name.objectname == "FixedRectangle" || name.objectname == "MoveableRectangle" || name.objectname == "Circle")
            //{
            //    Collider2D col = newObj.GetComponent<Collider2D>();
            //    PhysicsMaterial2D material = new PhysicsMaterial2D();
            //    material.friction = 0.4f;
            //    material.bounciness = 0.4f;
            //    col.sharedMaterial = material;
            //}
        }
        //otherwise do nothing;
    }
}
