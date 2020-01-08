using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonsControl : MonoBehaviour
{
    private static bool playState = false;
    private GameObject playButton;
    private GameObject stopButton;
    private GameObject replayButton;
    private GameObject clearButton;
    private GameObject leftBackButton;
    private GameObject rightBackButton;
    private GameObject objectsControlArea;
    private GameObject propertiesControlArea;
    private GameObject gravityButton;
    private GameObject gravityCheckBox;
    private Animator objectsControlAreaAnimator;
    private Animator propertiesControlAreaAnimator;

    [SerializeField] private Sprite checkedBoxSprite;
    [SerializeField] private Sprite uncheckedBoxSprite;

    private static bool initState = false;

    public void OnClickPlay ()
    {
        replayButton.SetActive(false);
        clearButton.SetActive(false);
        playState = !playState;
        ReplayControl.touchable = false;
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
        ReplayControl.touchable = true;
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
        
    }

    private void Awake()
    {
        ReplayControl.controlledByAnim = true;
        if (playButton == null)
        {
            playButton = GameObject.Find("PlayButton");
            stopButton = GameObject.Find("StopButton");
            replayButton = GameObject.Find("ReplayButton");
            clearButton = GameObject.Find("ClearButton");
            gravityButton = GameObject.Find("GravityButton");
            gravityCheckBox = gravityButton.transform.Find("Checkbox").gameObject;
            leftBackButton = GameObject.Find("LeftBackButton");
            rightBackButton = GameObject.Find("RightBackButton");
            objectsControlArea = GameObject.Find("ObjectsControlArea");
            propertiesControlArea = GameObject.Find("PropertiesControlArea");
            objectsControlAreaAnimator = objectsControlArea.GetComponent<Animator>();
            propertiesControlAreaAnimator = propertiesControlArea.GetComponent<Animator>();
        }
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

    //Other Buttons
    public void OnClickLeftBack()
    {
        //animation
        bool isShowing = objectsControlAreaAnimator.GetBool("isShowing");
        if (isShowing)
        {
            objectsControlAreaAnimator.SetBool("isShowing", false);
            objectsControlAreaAnimator.SetBool("isHiding", true);
        }
        else
        {
            objectsControlAreaAnimator.SetBool("isShowing", true);
            objectsControlAreaAnimator.SetBool("isHiding", false);
        }
    }

    public void OnClickRightBack()
    {
        //animation
        bool isShowing = propertiesControlAreaAnimator.GetBool("isShowing");
        if (isShowing)
        {
            propertiesControlAreaAnimator.SetBool("isShowing", false);
            propertiesControlAreaAnimator.SetBool("isHiding", true);
        }
        else
        {
            propertiesControlAreaAnimator.SetBool("isShowing", true);
            propertiesControlAreaAnimator.SetBool("isHiding", false);
        }
    }

    public void OnClickGravity()
    {
        GravityAffectable.gravityPresent = !GravityAffectable.gravityPresent;
        if (GravityAffectable.gravityPresent)
        {
            gravityCheckBox.GetComponent<Image>().sprite = checkedBoxSprite;
        }
        else
        {
            gravityCheckBox.GetComponent<Image>().sprite = uncheckedBoxSprite;
        }
        Debug.Log(GravityAffectable.gravityPresent);
    }
}
