using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attachable : MonoBehaviour
{
    //public static KDTree tree = new KDTree(2);
    public Transform trans;
    public static Attachable focused;

    public float widthMultiplier;
    public float heightMultiplier;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        //trans = this.GetComponent<Transform>();
        //tree.Insert(trans);
    }

    //Update is called once per frame
    void Update()
    {
        if (focused == this) this.gameObject.GetComponent<SpriteRenderer>().color = Util.shadedColor;
        else this.gameObject.GetComponent<SpriteRenderer>().color = Util.unshadedColor;
    }

    public void ShowSelected()
    {

    }

    public void HideSelected ()
    {

    }
}
