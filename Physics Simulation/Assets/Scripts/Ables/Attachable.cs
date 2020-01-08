using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attachable : MonoBehaviour
{
    //public static KDTree tree = new KDTree(2);
    public Transform trans;
    public Rigidbody2D rb;
    public static Attachable focused;

    public float widthMultiplier;
    public float heightMultiplier;

    public HashSet<Force> forces = new HashSet<Force>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        trans = this.GetComponent<Transform>();
        rb = this.GetComponent<Rigidbody2D>();
        //tree.Insert(trans);
    }

    private void FixedUpdate()
    {
        if (!rb.isKinematic)
        {
            foreach (Force force in forces)
            {
                rb.AddForce(force.normalizedDirection * force.magnitude, force.mode);
                //rb.AddForce(force.normalizedDirection * force.magnitude, force.mode);
            }
        }
        if (Util.OutOfBound(this.transform.position)) this.gameObject.GetComponent<Destructable>().Destruct();
    }

    public void RemoveForce (Force f)
    {
        forces.Remove(f);
    }

    public void AddForce (Force f)
    {
        forces.Add(f);
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
