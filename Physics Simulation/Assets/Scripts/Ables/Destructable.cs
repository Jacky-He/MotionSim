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
        Destroy(this.gameObject);
    }
}
