using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonsControl : MonoBehaviour
{
    private static bool playState = false;
    private GameObject playButton;
    private GameObject stopButton;
    private GameObject replayButton;
    private GameObject clearButton;

    private static bool initState = false;

    public void OnClickPlay ()
    {
        replayButton.SetActive(false);
        clearButton.SetActive(false);
        playState = !playState;
        //if not playing anymore, set everything to original position;
        if (!playState)
        {
            stopButton.SetActive(false);
            ReplayControl.recording = false;
            ReplayControl.resetting = true;
            ReplayControl.controlledByAnim = true;
        }
        else
        {
            stopButton.SetActive(true);
            ReplayControl.needsClearing = false;
            ReplayControl.resetting = false;
            ReplayControl.replaying = false;
            ReplayControl.sliderReplaying = false;
            ReplayControl.recording = true;
            ReplayControl.controlledByAnim = false;
        }
        //start the physics simulation;
    }

    public void OnClickStop ()
    {
        ReplayControl.recording = false;
        ReplayControl.sliderReplaying = true;
        ReplayControl.replaying = false;
        ReplayControl.controlledByAnim = true;
        replayButton.SetActive(true);
        clearButton.SetActive(true);
    }
    
    public void OnClickReplay()
    {
        ReplayControl.sliderReplaying = false;
        ReplayControl.replaying = true;
    }

    public void OnClickClear()
    {
        ReplayControl.needsClearing = true;
        replayButton.SetActive(false);
        clearButton.SetActive(false);
        stopButton.SetActive(false);
        playState = false;
        ReplayControl.recording = false;
        ReplayControl.resetting = false;
        ReplayControl.replaying = false;
        ReplayControl.sliderReplaying = false;
        ReplayControl.controlledByAnim = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        ReplayControl.controlledByAnim = true;
        playButton = GameObject.Find("PlayButton");
        stopButton = GameObject.Find("StopButton");
        replayButton = GameObject.Find("ReplayButton");
        clearButton = GameObject.Find("ClearButton");
    }

    // Update is called once per frame
    void Update()
    {
        if (!initState)
        {
            replayButton.SetActive(false);
            clearButton.SetActive(false);
            stopButton.SetActive(false);
            initState = true;
        }
    }
}
