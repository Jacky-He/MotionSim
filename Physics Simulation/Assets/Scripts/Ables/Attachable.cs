﻿using System.Collections;
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
    public HashSet<Velocity> velocities = new HashSet<Velocity>();
    public static HashSet<Collider2D> objectsCols = new HashSet<Collider2D>();

    bool flag = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnDestroy()
    {
        objectsCols.Remove(this.gameObject.GetComponent<Collider2D>());
    }

    private void Awake()
    {
        objectsCols.Add(this.gameObject.GetComponent<Collider2D>());
        trans = this.GetComponent<Transform>();
        rb = this.GetComponent<Rigidbody2D>();
        //tree.Insert(trans);
    }

    private void FixedUpdate()
    {
        if (!rb.isKinematic)
        {
            foreach (Force force in forces) rb.AddForce(force.normalizedDirection * force.magnitude, force.mode);
            if (!flag)
            {
                Vector3 res = Vector3.zero;
                foreach (Velocity each in velocities) res += each.velocity;
                rb.velocity = res;
                flag = true;
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

    public void AddVelocity (Velocity v)
    { 
        velocities.Add(v);
    }

    public void RemoveVelocity (Velocity v)
    {
        velocities.Remove(v);
    }

    //Update is called once per frame
    void Update()
    {
        if (focused == this) this.gameObject.GetComponent<SpriteRenderer>().color = Util.shadedColor;
        else this.gameObject.GetComponent<SpriteRenderer>().color = Util.unshadedColor;
    }

    public static void ResetAllColliders ()
    {
        foreach (Collider2D col in objectsCols)
        {
            if (col != null)
            {
                col.enabled = false;
                col.enabled = true;
            }
        }
    }

    public void ShowSelected()
    {

    }

    public void HideSelected ()
    {

    }
}
