using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonsControl : MonoBehaviour
{
    private static bool playState = false;
    private static GameObject playButton;
    private static GameObject stopButton;
    private static GameObject replayButton;
    private static GameObject clearButton;
    private static GameObject leftBackButton;
    private static GameObject rightBackButton;
    private static GameObject objectsControlArea;
    private static GameObject propertiesControlArea;
    private static GameObject gravityButton;
    private static GameObject gravityCheckBox;
    private Animator objectsControlAreaAnimator;
    private Animator propertiesControlAreaAnimator;

    private static GameObject earthImage;

    [SerializeField] private Sprite checkedBoxSprite;
    [SerializeField] private Sprite uncheckedBoxSprite;

    private static bool initState = false;

    public void OnClickPlay ()
    {
        replayButton.SetActive(false);
        clearButton.SetActive(false);
        playState = !playState;
        //ReplayControl.touchable = false;
        //if not playing anymore, set everything to original position;
        if (!playState)
        {
            playButton.GetComponent<Image>().color = Util.ReplayButtonsOnColor;
            stopButton.SetActive(false);
            ReplayControl.recording = false;
            ReplayControl.resetting = true;
            ReplayControl.controlledByAnim = true;
            ReplayControl.touchable = true;
        }
        else
        {
            playButton.GetComponent<Image>().color = Util.ReplayButtonsOffColor;
            stopButton.SetActive(true);
            ReplayControl.resetting = false;
            ReplayControl.replaying = false;
            ReplayControl.sliderReplaying = false;
            ReplayControl.recording = true;
            ReplayControl.controlledByAnim = false;
            ReplayControl.touchable = false;
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
        ReplayControl.totalPointsCnt = 0;
    }

    public static void StaticStop ()
    {
        ReplayControl.recording = false;
        ReplayControl.sliderReplaying = true;
        ReplayControl.replaying = false;
        ReplayControl.controlledByAnim = true;
        replayButton.SetActive(true);
        clearButton.SetActive(true);
        ReplayControl.totalPointsCnt = 0;
    }

    public void OnClickReplay()
    {
        ReplayControl.sliderReplaying = false;
        ReplayControl.replaying = true;
    }

    public void OnClickClear()
    {
        ReplayControl.touchable = true;
        //starts Clearing stuff
        Destructable.ClearAll();
        //ends Clearing stuff
        replayButton.SetActive(false);
        clearButton.SetActive(false);
        stopButton.SetActive(false);
        playState = false;
        playButton.GetComponent<Image>().color = Util.ReplayButtonsOnColor;
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
            earthImage = GameObject.Find("EarthImage");
            //Debug.Log(earthImage);
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
        bool isShowing = ObjectsControlScript.objectsControlAreaShowing;
        if (isShowing)
        {
            ObjectsControlScript.objectsControlAreaShowing = false;
            leftBackButton.GetComponent<Image>().color = Util.LeftBackOffColor;
        }
        else
        {
            ObjectsControlScript.objectsControlAreaShowing = true;
            leftBackButton.GetComponent<Image>().color = Util.LeftBackOnColor;
        }

        //bool isShowing = objectsControlAreaAnimator.GetBool("isShowing");
        //if (isShowing)
        //{
        //    objectsControlAreaAnimator.SetBool("isShowing", false);
        //    objectsControlAreaAnimator.SetBool("isHiding", true);
        //    leftBackButton.GetComponent<Image>().color = Util.LeftBackOffColor;
        //}
        //else
        //{
        //    objectsControlAreaAnimator.SetBool("isShowing", true);
        //    objectsControlAreaAnimator.SetBool("isHiding", false);
        //    leftBackButton.GetComponent<Image>().color = Util.LeftBackOnColor;
        //}
    }

    public void OnClickRightBack()
    {
        bool isShowing = PropertiesControlAreaScript.propertiesControlAreaShowing;
        if (isShowing)
        {
            PropertiesControlAreaScript.propertiesControlAreaShowing = false;
            rightBackButton.GetComponent<Image>().color = Util.RightBackOffColor;
        }
        else
        {
            PropertiesControlAreaScript.propertiesControlAreaShowing = true;
            rightBackButton.GetComponent<Image>().color = Util.RightBackOnColor;
        }

        //animation
        //bool isShowing = propertiesControlAreaAnimator.GetBool("isShowing");
        //if (isShowing)
        //{
        //    propertiesControlAreaAnimator.SetBool("isShowing", false);
        //    propertiesControlAreaAnimator.SetBool("isHiding", true);
        //    rightBackButton.GetComponent<Image>().color = Util.RightBackOffColor;
        //}
        //else
        //{
        //    propertiesControlAreaAnimator.SetBool("isShowing", true);
        //    propertiesControlAreaAnimator.SetBool("isHiding", false);
        //    rightBackButton.GetComponent<Image>().color = Util.RightBackOnColor;
        //}
    }

    public void OnClickGravity()
    {
        GravityAffectable.gravityPresent = !GravityAffectable.gravityPresent;
        if (GravityAffectable.gravityPresent)
        {
            gravityCheckBox.GetComponent<Image>().sprite = checkedBoxSprite;
            earthImage.SetActive(true);
        }
        else
        {
            gravityCheckBox.GetComponent<Image>().sprite = uncheckedBoxSprite;
            earthImage.SetActive(false);
        }
    }

    public void OnClickGraphTab ()
    {

    }

    public void OnClickPropertiesTab()
    {

    }
}
