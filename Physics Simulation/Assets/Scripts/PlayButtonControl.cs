﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayButtonControl : MonoBehaviour
{

    public void OnClick()
    {
        //starts the physics
        Physics2D.autoSimulation = true;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        //stops the physics
        Physics2D.autoSimulation = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}