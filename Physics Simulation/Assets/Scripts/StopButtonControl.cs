using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopButtonControl : MonoBehaviour
{
    public void OnClick()
    {
        Physics2D.autoSimulation = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
