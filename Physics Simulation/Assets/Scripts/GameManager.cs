using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Uses SingleTon Pattern to manage the game

public class GameManager
{
    private static GameManager instance = new GameManager();

    private GameManager()
    {
        //initialize your game manager here. Do to reference to GameObjects (i.e. GameObject.Find etc.)
        //because the game manager will be created before the objects
    }

    public static GameManager Instance
    {
        get { return instance; }
    }

    //Add your game manager members here
    public void Pause (bool paused)
    {

    }
}
