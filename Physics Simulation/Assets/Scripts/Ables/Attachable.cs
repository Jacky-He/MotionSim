using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attachable : MonoBehaviour
{
    public static KDTree tree = new KDTree(2);
    public Transform trans;
    public static Attachable selected;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        trans = this.GetComponent<Transform>();
        tree.Insert(trans);
    }

    // Update is called once per frame
    void Update()
    {
        tree.Replace(trans);
        if (selected != this)
        {
            HideSelected();
        }
    }

    public void ShowSelected ()
    {
        selected = this;
    }

    public void HideSelected ()
    {

    }
}
