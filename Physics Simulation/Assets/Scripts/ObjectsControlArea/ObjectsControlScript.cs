using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using v2 = UnityEngine.Vector2;
using v3 = UnityEngine.Vector3;
using go = UnityEngine.GameObject;
using rtm = UnityEngine.RectTransform;
using tm = UnityEngine.Transform;

public class ObjectsControlScript: MonoBehaviour
{
    // Use this for initialization
    private go canvas;
    private go objectsControlArea;
    private rtm objectsControlAreaRTM;
    private go propertiesControlArea;

    //private static bool propertiesControlAreaShown = true;
    public static bool objectsControlAreaShowing = true;

    //private static float propertiesControlAreaAnimationTime = 1f; //seconds
    public static float objectsControlAreaAnimationTime = 0.2f; //seconds

    //private static float propertiesControlAreaAnimationShift = 1f;
    public static float objectsControlAreaAnimationShift = 1f;

    public static float objectsControlAreaOutXpos;
    public static float objectsControlAreaInXpos;

    private void Awake()
    {
        canvas = go.Find("Canvas");
        objectsControlArea = go.Find("ObjectsControlArea");
        propertiesControlArea = go.Find("PropertiesControlArea");
        objectsControlAreaRTM = objectsControlArea.GetComponent<rtm>();
    }

    void Start()
    {
        ApplySafeArea();
    }
    

    // Update is called once per frame
    void Update()
    {
        if (objectsControlAreaShowing)
        {
            v3 oldpos = objectsControlAreaRTM.anchoredPosition;
            if (oldpos.x >= objectsControlAreaOutXpos) objectsControlAreaRTM.anchoredPosition = new v2(objectsControlAreaOutXpos, oldpos.y);
            objectsControlAreaRTM.anchoredPosition = new v2(Mathf.Min(oldpos.x + objectsControlAreaAnimationShift/objectsControlAreaAnimationTime*Time.deltaTime, objectsControlAreaOutXpos), oldpos.y);
        }
        else
        {
            v3 oldpos = objectsControlAreaRTM.anchoredPosition;
            if (oldpos.x <= objectsControlAreaInXpos) objectsControlAreaRTM.anchoredPosition = new v2(objectsControlAreaInXpos, oldpos.y);
            objectsControlAreaRTM.anchoredPosition = new v2(Mathf.Max(oldpos.x - objectsControlAreaAnimationShift / objectsControlAreaAnimationTime * Time.deltaTime, objectsControlAreaInXpos), oldpos.y);
        }
    }

    private void ApplySafeArea()
    {

        Rect safeArea = Screen.safeArea;

        v3 safeAreaPos = safeArea.position / canvas.transform.localScale.x;

        objectsControlAreaOutXpos = Application.isMobilePlatform ? safeAreaPos.x : 0f;

        objectsControlAreaInXpos = -272f;

        objectsControlAreaAnimationShift = Mathf.Abs(objectsControlAreaOutXpos - objectsControlAreaInXpos);
    }
}
