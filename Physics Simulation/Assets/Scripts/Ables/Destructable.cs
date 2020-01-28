using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour
{
    public static HashSet<GameObject> objects = new HashSet<GameObject>();

    public static void ClearAll()
    {
        foreach (GameObject each in objects)
        {
            PropertiesEditable temp = each.GetComponent<PropertiesEditable>();
            if (temp != null) temp.Destruct();
            Destroy(each);
        }
        objects.Clear();
    }

    private void Awake()
    {
        objects.Add(this.gameObject);
    }

    public void Destruct ()
    {
        objects.Remove(this.gameObject);
        //deletes the generated lines and labels
        PropertiesEditable temp = this.gameObject.GetComponent<PropertiesEditable>();
        if (temp != null) temp.Destruct();
        Destroy(this.gameObject);
    }
}
