using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util: MonoBehaviour
{
    private static List<GameObject> allSprites = new List<GameObject>();

    private static Dictionary<GameObject, List <Vector3>> posStore = new Dictionary <GameObject, List <Vector3>>();

    public static void AddSprite (GameObject obj)
    {

        allSprites.Add(obj);
    }

    public static void clearAllSprites ()
    {
        allSprites.Clear();
    }

    void FixedUpdate()
    {
        foreach (GameObject obj in allSprites)
        {
            if (posStore[obj] == null) posStore[obj] = new List<Vector3>();
            posStore[obj].Add(obj.transform.position);
        }
    }

    void Update()
    {
        Debug.Log("called");
    }
}