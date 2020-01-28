using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using v2 = UnityEngine.Vector2;
using v3 = UnityEngine.Vector3;
using go = UnityEngine.GameObject;
using rtm = UnityEngine.RectTransform;
using tm = UnityEngine.Transform;

public class PropertiesAreaScript: MonoBehaviour
{

    private const int numProperties = 8;

    private string[] propertiesNames = new string[numProperties] { "Mass (kg)", "Length (m)", "Width (m)", "Diameter (m)", "Spring Constant (N/m)", "Force (N)", "Velocity (m/s)", "Angle (°}" };

    Text[] labels = new Text[numProperties];
    InputField[] inputFields = new InputField[numProperties];
    Slider[] sliders = new Slider[numProperties];

    public go scrollContent;
    public go sliderTemplate;
    public go labelTemplate;
    public go inputFieldTemplate;

    private go canvass;
    private int currmask;

    private Rigidbody2D rbcurr
    {
        get { return PropertiesControlAreaScript.focusedObject == null ? null : PropertiesControlAreaScript.focusedObject.GetComponent<Rigidbody2D>(); }
    }
    private tm transcurr
    {
        get { return PropertiesControlAreaScript.focusedObject == null ? null : PropertiesControlAreaScript.focusedObject.GetComponent<tm>(); }
    }
    private string namecurr
    {
        get { return PropertiesControlAreaScript.focusedObject == null ? "" : PropertiesControlAreaScript.focusedObject.GetComponent<Name>().objectname;}
    }
    private go focused
    {
        get { return PropertiesControlAreaScript.focusedObject; }
    }

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (namecurr == "") SetNeededProperties(0);
        else if (namecurr == "FixedRectangle") SetNeededProperties((1 << 1) + (1 << 2) + (1 << 7));
        else if (namecurr == "MoveableRectangle") SetNeededProperties(1 + (1 << 1) + (1 << 2) + (1 << 7));
        else if (namecurr == "Circle") SetNeededProperties(1 + (1 << 3));
        else if (namecurr == "Force") SetNeededProperties((1 << 5));
        else if (namecurr == "Velocity") SetNeededProperties((1 << 6));
    }

    private void SetNeededProperties(int mask)
    {
        if (currmask == mask) return;
        currmask = mask;
        for (int i = 0; i < numProperties; i++)
        {
            labels[i].gameObject.SetActive(false);
            labels[i].gameObject.transform.SetParent(canvass.transform);
            inputFields[i].gameObject.SetActive(false);
            inputFields[i].gameObject.transform.SetParent(canvass.transform);
            sliders[i].gameObject.SetActive(false);
            sliders[i].gameObject.transform.SetParent(canvass.transform);
        }
        for (int i = 0, tempmask = mask;  tempmask != 0; i++, tempmask >>= 1)
        {
            if ((tempmask&1) == 1)
            {
                labels[i].gameObject.transform.SetParent(scrollContent.transform);
                labels[i].gameObject.SetActive(true);
                inputFields[i].gameObject.transform.SetParent(scrollContent.transform);
                inputFields[i].gameObject.SetActive(true);
                sliders[i].gameObject.transform.SetParent(scrollContent.transform);
                sliders[i].gameObject.SetActive(true);
            }
        }
        AdjustValues();
    }

    private void Awake()
    {
        canvass = go.Find("Canvas");
        for (int i = 0; i < numProperties; i++)
        {
            int j = i;
            go obj1 = Instantiate(labelTemplate) as go;
            obj1.SetActive(false);
            obj1.transform.SetParent(scrollContent.transform, false);
            Text text = obj1.GetComponent<Text>();
            text.color = Color.black;
            text.text = propertiesNames[i];
            labels[i] = text;

            go obj2 = Instantiate(inputFieldTemplate) as go;
            obj2.SetActive(false);
            obj2.transform.SetParent(scrollContent.transform, false);
            InputField field = obj2.GetComponent<InputField>();
            field.onValueChanged.AddListener(delegate { this.fieldValueChange(j); });
            field.onEndEdit.AddListener(delegate { this.fieldEndEdit(j); });
            inputFields[i] = field;

            go obj3 = Instantiate(sliderTemplate) as go;
            obj3.SetActive(false);
            obj3.transform.SetParent(scrollContent.transform, false);
            Slider s = obj3.GetComponent<Slider>();
            s.onValueChanged.AddListener(delegate { this.sliderValueChange(j); });
            sliders[i] = s;
        }
    }

    private void AdjustValues()
    {
        for (int i = 0, tempmask = currmask; tempmask != 0; i++, tempmask >>= 1)
        {
            if ((tempmask & 1) != 1) continue;
            if (i == 0)
            {
                (float, float) bounds = (0f, 100f);
                sliders[i].SetValueWithoutNotify ((rbcurr.mass - bounds.Item1)/bounds.Item2);
                inputFields[i].SetTextWithoutNotify("" + Mathf.Round(rbcurr.mass*100f)/100f); 
            }
            else if (i == 1)
            {
                (float, float) bounds = (0f, 100f);
                float length = 0f;
                if (namecurr == "FixedRectangle") length = transcurr.localScale.x * Util.FixedRectWidthMultiplier;
                else if (namecurr == "MoveableRectangle") length = transcurr.localScale.x * Util.MoveableRectWidthMultiplier;
                sliders[i].SetValueWithoutNotify((length - bounds.Item1) / bounds.Item2);
                inputFields[i].SetTextWithoutNotify("" + Mathf.Round(length * 100f) / 100f);
            }
            else if (i == 2)
            {
                (float, float) bounds = (0f, 100f);
                float width = 0f;
                if (namecurr == "FixedRectangle") width = transcurr.localScale.y * Util.FixedRectHeightMultiplier;
                else if (namecurr == "MoveableRectangle") width = transcurr.localScale.y * Util.MoveableRectHeightMultiplier;
                sliders[i].SetValueWithoutNotify((width - bounds.Item1) / bounds.Item2);
                inputFields[i].SetTextWithoutNotify("" + Mathf.Round(width * 100f) / 100f);
            }
            else if (i == 3) //circle
            {
                (float, float) bounds = (0f, 100f);
                float diameter = transcurr.localScale.x;
                sliders[i].SetValueWithoutNotify((diameter - bounds.Item1) / bounds.Item2);
                inputFields[i].SetTextWithoutNotify("" + Mathf.Round(diameter * 100f) / 100f);
            }
            else if (i == 7)
            {
                (float, float) bounds = (0f, 359f);
                float angle = transcurr.localEulerAngles.z;
                sliders[i].SetValueWithoutNotify((angle - bounds.Item1) / bounds.Item2);
                inputFields[i].SetTextWithoutNotify("" + Mathf.Round(angle * 100f) / 100f);
            }
        }
    }

    public void sliderValueChange (int idx)
    {
       // Debug.Log(idx);
        float value = sliders[idx].value;
        if (idx == 0)
        {
            (float, float) bounds = (0f, 100f);
            float mass = bounds.Item1 + value / 1f * (bounds.Item2 - bounds.Item1);
            rbcurr.mass = mass;
            inputFields[idx].SetTextWithoutNotify("" + Mathf.Round(mass * 100f) / 100f);
        }
        else if (idx == 1)
        {
            (float, float) bounds = (0f, 100f);
            float length = bounds.Item1 + value / 1f * (bounds.Item2 - bounds.Item1);
            if (namecurr == "FixedRectangle") transcurr.localScale = new v2(length / Util.FixedRectWidthMultiplier, transcurr.localScale.y);
            else if (namecurr == "MoveableRectangle") transcurr.localScale = new v2(length / Util.MoveableRectWidthMultiplier, transcurr.localScale.y);
            inputFields[idx].SetTextWithoutNotify("" + Mathf.Round(length * 100f) / 100f);
        }
        else if (idx == 2)
        {
            (float, float) bounds = (0f, 100f);
            float width = bounds.Item1 + value / 1f * (bounds.Item2 - bounds.Item1);
            if (namecurr == "FixedRectangle") transcurr.localScale = new v2(transcurr.localScale.x, width / Util.FixedRectHeightMultiplier);
            else if (namecurr == "MoveableRectangle") transcurr.localScale = new v2(transcurr.localScale.x, width / Util.MoveableRectHeightMultiplier);
            inputFields[idx].SetTextWithoutNotify("" + Mathf.Round(width * 100f) / 100f);
        }
        else if (idx == 3) //circle
        {
            (float, float) bounds = (0f, 100f);
            float diameter = bounds.Item1 + value / 1f * (bounds.Item2 - bounds.Item1);
            transcurr.localScale = new v2(diameter, diameter);
            inputFields[idx].SetTextWithoutNotify("" + Mathf.Round(diameter * 100f) / 100f);
        }
        else if (idx == 7)
        {
            (float, float) bounds = (0f, 359f);
            float angle = bounds.Item1 + value / 1f * (bounds.Item2 - bounds.Item1);
            transcurr.localEulerAngles = new v3(0, 0, angle);
            inputFields[idx].SetTextWithoutNotify("" + Mathf.Round(angle * 100f) / 100f);
        }
    }

    public void fieldValueChange(int idx)
    {
        float value;
        if (!float.TryParse(inputFields[idx].text, out value)) return;
        if (idx == 0)
        {
            (float, float) bounds = (0f, 100f);
            value = Mathf.Clamp(value, bounds.Item1, bounds.Item2);
            rbcurr.mass = value;
            sliders[idx].SetValueWithoutNotify((value - bounds.Item1)/bounds.Item2);
            inputFields[idx].SetTextWithoutNotify("" + Mathf.Round(value*100f)/100f);
        }
        else if (idx == 1)
        {
            (float, float) bounds = (0f, 100f);
            value = Mathf.Clamp(value, bounds.Item1, bounds.Item2);
            float length = value;
            if (namecurr == "FixedRectangle") transcurr.localScale = new v2(length / Util.FixedRectWidthMultiplier, transcurr.localScale.y);
            else if (namecurr == "MoveableRectangle") transcurr.localScale = new v2(length / Util.MoveableRectWidthMultiplier, transcurr.localScale.y);
            sliders[idx].SetValueWithoutNotify((length - bounds.Item1) / bounds.Item2);
            inputFields[idx].SetTextWithoutNotify("" + Mathf.Round(length * 100f) / 100f);
        }
        else if (idx == 2)
        {
            (float, float) bounds = (0f, 100f);
            value = Mathf.Clamp(value, bounds.Item1, bounds.Item2);
            float width = value;
            if (namecurr == "FixedRectangle") transcurr.localScale = new v2(transcurr.localScale.x, width / Util.FixedRectHeightMultiplier);
            else if (namecurr == "MoveableRectangle") transcurr.localScale = new v2(transcurr.localScale.x, width / Util.MoveableRectHeightMultiplier);
            sliders[idx].SetValueWithoutNotify((width - bounds.Item1)/bounds.Item2);
            inputFields[idx].SetTextWithoutNotify("" + Mathf.Round(width * 100f) / 100f);
        }
        else if(idx == 3) //circle
        {
            (float, float) bounds = (0f, 100f);
            value = Mathf.Clamp(value, bounds.Item1, bounds.Item2);
            transcurr.localScale = new v2(value, value);
            sliders[idx].SetValueWithoutNotify((value - bounds.Item1)/bounds.Item2);
            inputFields[idx].SetTextWithoutNotify("" + Mathf.Round(value * 100f) / 100f);
        }
        else if (idx == 7)
        {
            (float, float) bounds = (0f, 359f);
            value = Mathf.Clamp(value, bounds.Item1, bounds.Item2);
            transcurr.localEulerAngles = new v3(0, 0, value);
            sliders[idx].SetValueWithoutNotify((value - bounds.Item1) / bounds.Item2);
            inputFields[idx].SetTextWithoutNotify("" + Mathf.Round(value * 100f) / 100f);
        }
    }

    public void fieldEndEdit(int idx)
    {

    }

}
