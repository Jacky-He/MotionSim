using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using v2 = UnityEngine.Vector2;
using v3 = UnityEngine.Vector3;
using go = UnityEngine.GameObject;
using rtm = UnityEngine.RectTransform;

public class PropertiesAreaScript: MonoBehaviour
{

    private const int numProperties = 8;

    private string[] propertiesNames = new string[numProperties] { "Mass (kg)", "Length (m)", "Width (m)", "Diameter (m)", "Spring Constant (N/m)", "Force (N)", "Velocity (m/s)", "Angle (°}" };

    go[] labels = new go[numProperties];
    go[] inputFields = new go[numProperties];
    go[] sliders = new go[numProperties];

    public go scrollContent;
    public go sliderTemplate;
    public go labelTemplate;
    public go inputFieldTemplate;

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Awake()
    {
        for (int i = 0; i < numProperties; i++)
        {
            go obj1 = Instantiate(labelTemplate) as go;
            obj1.SetActive(true);
            obj1.transform.SetParent(scrollContent.transform, false);
            Text text = obj1.GetComponent<Text>();
            text.color = Color.black;
            text.text = propertiesNames[i];
            labels[i] = obj1;

            go obj2 = Instantiate(inputFieldTemplate) as go;
            obj2.SetActive(true);
            obj2.transform.SetParent(scrollContent.transform, false);
            InputField field = obj2.GetComponent<InputField>();
            field.onValueChanged.AddListener(delegate { this.fieldValueChange(i); });
            field.onEndEdit.AddListener(delegate { this.fieldEndEdit(i); });
            inputFields[i] = obj2;

            go obj3 = Instantiate(sliderTemplate) as go;
            obj3.SetActive(true);
            obj3.transform.SetParent(scrollContent.transform, false);
            Slider s = obj3.GetComponent<Slider>();
            s.onValueChanged.AddListener(delegate { this.sliderValueChange(i); });
            sliders[i] = obj3;
        }
    }

    public void sliderValueChange (int idx)
    {

    }

    public void fieldValueChange(int idx)
    {

    }

    public void fieldEndEdit(int idx)
    {

    }

}
