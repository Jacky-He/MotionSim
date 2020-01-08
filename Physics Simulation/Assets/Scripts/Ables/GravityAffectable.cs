using UnityEngine;
using System.Collections;

public class GravityAffectable : MonoBehaviour
{
    public static bool gravityPresent = true;

    private Rigidbody2D rb;

    // Use this for initialization
    void Start()
    {
        
    }

    private void Awake()
    {
        rb = this.gameObject.GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityPresent ? 100f : 0f;
    }

    private void FixedUpdate()
    {
        //change gravity
        rb.gravityScale = gravityPresent ? 100f : 0f;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
