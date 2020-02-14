using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using v2 = UnityEngine.Vector2;
using v3 = UnityEngine.Vector3;
using go = UnityEngine.GameObject;
using rtm = UnityEngine.RectTransform;
using tm = UnityEngine.Transform;
using System;

public class PropertiesAreaScript: MonoBehaviour
{

    private const int numProperties = 11;
    //possible idea: if the force rotates with the objects then you can simulate uniform circular motion;
    private string[] propertiesNames = new string[numProperties] { "Mass (kg)", "Length (m)", "Width (m)", "Diameter (m)", "Spring Constant (N/m)", "Force (N)", "Speed (m/s)", "Angle (°)", "Equilibrium Length (m)", "Break Force (N)", "Coefficient of Kinetic Friction (Global)"};

    Text[] labels = new Text[numProperties];
    InputField[] inputFields = new InputField[numProperties];
    Slider[] sliders = new Slider[numProperties];
    go deleteButton;

    public go scrollContent;
    public go sliderTemplate;
    public go labelTemplate;
    public go inputFieldTemplate;
    public go deleteButtonTemplate;

    private go canvass;
    private int currmask;
    private go prevObject;

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
        else if (namecurr == "FixedRectangle") SetNeededProperties((1 << 1) + (1 << 2) + (1 << 7) + (1 << 10));
        else if (namecurr == "MoveableRectangle") SetNeededProperties(1 + (1 << 1) + (1 << 2) + (1 << 7) + (1 << 10));
        else if (namecurr == "Circle") SetNeededProperties(1 + (1 << 3) + (1 << 10));
        else if (namecurr == "Force") SetNeededProperties((1 << 5) + (1 << 7));
        else if (namecurr == "Velocity") SetNeededProperties((1 << 6) + (1 << 7));
        else if (namecurr == "Spring") SetNeededProperties((1 << 4) + (1 << 8) + (1 << 9));
    }

    private void SetNeededProperties(int mask)
    {
        if (currmask == mask && prevObject == this.focused) return;
        currmask = mask;
        prevObject = this.focused;
        for (int i = 0; i < numProperties; i++)
        {
            labels[i].gameObject.SetActive(false);
            labels[i].gameObject.transform.SetParent(canvass.transform);
            inputFields[i].gameObject.SetActive(false);
            inputFields[i].gameObject.transform.SetParent(canvass.transform);
            sliders[i].gameObject.SetActive(false);
            sliders[i].gameObject.transform.SetParent(canvass.transform);
        }
        deleteButton.SetActive(false);
        deleteButton.transform.SetParent(canvass.transform);
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
        if(mask != 0)
        {
            deleteButton.transform.SetParent(scrollContent.transform);
            deleteButton.SetActive(true);
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
            text.color = Color.white;
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
        go obj4 = Instantiate(deleteButtonTemplate) as go;
        obj4.SetActive(false);
        obj4.transform.SetParent(scrollContent.transform, false);
        Button button = obj4.GetComponent<Button>();
        button.onClick.AddListener(delegate { this.Delete(); });
        deleteButton = obj4;
    }

    public void CoerceAdjustValues(int i)
    {
        if (i == 5)
        {
            (float, float) bounds = (0.01f, 1000f);
            ForceControl force = focused.GetComponent<ForceControl>();
            if (force == null) return;
            float newtons = Mathf.Clamp(force.getForce(), bounds.Item1, bounds.Item2);
            sliders[i].SetValueWithoutNotify((newtons - bounds.Item1) / bounds.Item2);
            inputFields[i].SetTextWithoutNotify("" + Mathf.Round(newtons * 100f) / 100f);
           // Debug.Log(sliders[i].value);
        }
        else if (i == 6)
        {
            (float, float) bounds = (0.01f, 100f);
            VelocityControl velocity = focused.GetComponent<VelocityControl>();
            if (velocity == null) return;
            float speed = Mathf.Clamp(velocity.getSpeed(), bounds.Item1, bounds.Item2);
            sliders[i].SetValueWithoutNotify((speed - bounds.Item1) / bounds.Item2);
            inputFields[i].SetTextWithoutNotify("" + Mathf.Round(speed * 100f) / 100f);
        }
        else if (i == 7)
        {
            (float, float) bounds = (0f, 360f);
            float angle;
            if (namecurr == "Force")
            {
                ForceControl force = focused.GetComponent<ForceControl>();
                if (force == null) return;
                angle = Mathf.Clamp(force.getAngle(), bounds.Item1, bounds.Item2);
            }
            else if (namecurr == "Velocity")
            {
                VelocityControl velocity = focused.GetComponent<VelocityControl>();
                if (velocity == null) return;
                angle = Mathf.Clamp(velocity.getAngle(), bounds.Item1, bounds.Item2);
            }
            else
            {
                angle = Mathf.Clamp(transcurr.localEulerAngles.z, bounds.Item1, bounds.Item2);
            }
            sliders[i].SetValueWithoutNotify((angle - bounds.Item1) / bounds.Item2);
            inputFields[i].SetTextWithoutNotify("" + Mathf.Round(angle * 100f) / 100f);
        }
        else if (i == 8) //spring equilibrium length
        {

        }
    }

    private void AdjustValues()
    {
        for (int i = 0, tempmask = currmask; tempmask != 0; i++, tempmask >>= 1)
        {
            if ((tempmask & 1) != 1) continue;
            if (i == 0)
            {
                (float, float) bounds = (0.01f, 1000f);
                sliders[i].SetValueWithoutNotify ((rbcurr.mass - bounds.Item1)/bounds.Item2);
                inputFields[i].SetTextWithoutNotify("" + Mathf.Round(rbcurr.mass*100f)/100f); 
            }
            else if (i == 1)
            {
                (float, float) bounds = (0.01f, 1000f);
                float length = 0f;
                if (namecurr == "FixedRectangle") length = transcurr.localScale.x * Util.FixedRectWidthMultiplier;
                else if (namecurr == "MoveableRectangle") length = transcurr.localScale.x * Util.MoveableRectWidthMultiplier;
                sliders[i].SetValueWithoutNotify((length - bounds.Item1) / bounds.Item2);
                inputFields[i].SetTextWithoutNotify("" + Mathf.Round(length * 100f) / 100f);
            }
            else if (i == 2)
            {
                (float, float) bounds = (0.01f, 1000f);
                float width = 0f;
                if (namecurr == "FixedRectangle") width = transcurr.localScale.y * Util.FixedRectHeightMultiplier;
                else if (namecurr == "MoveableRectangle") width = transcurr.localScale.y * Util.MoveableRectHeightMultiplier;
                sliders[i].SetValueWithoutNotify((width - bounds.Item1) / bounds.Item2);
                inputFields[i].SetTextWithoutNotify("" + Mathf.Round(width * 100f) / 100f);
            }
            else if (i == 3) //circle
            {
                (float, float) bounds = (0.01f, 100f);
                float diameter = transcurr.localScale.x;
                sliders[i].SetValueWithoutNotify((diameter - bounds.Item1) / bounds.Item2);
                inputFields[i].SetTextWithoutNotify("" + Mathf.Round(diameter * 100f) / 100f);
            }
            else if (i == 4) //spring constant (N/m)
            {
                (float, float) bounds = (0.01f, 1000f);
                SpringControl spring = focused.GetComponent<SpringControl>();
                float constant = Mathf.Clamp(spring.getSpringConstant(), bounds.Item1, bounds.Item2);
                sliders[i].SetValueWithoutNotify((constant - bounds.Item1) / bounds.Item2);
                inputFields[i].SetTextWithoutNotify("" + Mathf.Round(constant * 100f) / 100f);
            }
            else if (i == 5) //force
            {
                (float, float) bounds = (0.01f, 1000f);
                ForceControl force = focused.GetComponent<ForceControl>();
                float newtons = Mathf.Clamp(force.getForce(), bounds.Item1, bounds.Item2);
                sliders[i].SetValueWithoutNotify((newtons - bounds.Item1) / bounds.Item2);
                inputFields[i].SetTextWithoutNotify("" + Mathf.Round(newtons * 100f) / 100f);
            }
            else if (i == 6) //velocity
            {
                (float, float) bounds = (0.01f, 100f);
                VelocityControl velocity = focused.GetComponent<VelocityControl>();
                float speed = Mathf.Clamp(velocity.getSpeed(), bounds.Item1, bounds.Item2);
                sliders[i].SetValueWithoutNotify((speed - bounds.Item1) / bounds.Item2);
                inputFields[i].SetTextWithoutNotify("" + Mathf.Round(speed * 100f) / 100f);
            }
            else if (i == 7)
            {
                (float, float) bounds = (0f, 360f);
                float angle = transcurr.localEulerAngles.z;
                if (namecurr == "Force")
                {
                    ForceControl force = focused.GetComponent<ForceControl>();
                    if (force == null) return;
                    angle = Mathf.Clamp(force.getAngle(), bounds.Item1, bounds.Item2);
                }
                else if (namecurr == "Velocity")
                {
                    VelocityControl velocity = focused.GetComponent<VelocityControl>();
                    if (velocity == null) return;
                    angle = Mathf.Clamp(velocity.getAngle(), bounds.Item1, bounds.Item2);
                }
                else
                {
                    angle = Mathf.Clamp (transcurr.localEulerAngles.z, bounds.Item1, bounds.Item2);
                }
                sliders[i].SetValueWithoutNotify((angle - bounds.Item1) / bounds.Item2);
                inputFields[i].SetTextWithoutNotify("" + Mathf.Round(angle * 100f) / 100f);
            }
            else if (i == 8) //equilibrium length
            {
                //cannot set this property when spring is attached
                SpringControl spring = focused.GetComponent<SpringControl>();
                //if (!spring.available()) { this.CoerceAdjustValues(8); return; }
                (float, float) bounds = (0.01f, 100f);
                float elength = Mathf.Clamp(spring.getElength(), bounds.Item1, bounds.Item2);
                sliders[i].SetValueWithoutNotify((elength - bounds.Item1) / bounds.Item2);
                inputFields[i].SetTextWithoutNotify("" + Mathf.Round(elength * 100f) / 100f);
            }
            else if (i == 9) //break force (N)
            {
                (double, double) bounds = (0.01, 999999999);
                SpringControl spring = focused.GetComponent<SpringControl>();
                double breakforce = Math.Max(Math.Min(spring.getBreakForce(), bounds.Item2), bounds.Item1);
                sliders[i].SetValueWithoutNotify((float)((breakforce - bounds.Item1) / bounds.Item2));
                inputFields[i].SetTextWithoutNotify("" + Math.Round(breakforce * 100f) / 100f);
            }
            else if (i == 10)
            {
                (float, float) bounds = (0f, 1f);
                Collider2D col = focused.GetComponent<Collider2D>();
                sliders[i].SetValueWithoutNotify((col.sharedMaterial.friction - bounds.Item1) / bounds.Item2);
                inputFields[i].SetTextWithoutNotify("" + Mathf.Round(col.sharedMaterial.friction * 100f) / 100f);
            }
            //else if (i == 10)
            //{
            //    (float, float) bounds = (0f, 1f);
            //    Collider2D col = focused.GetComponent<Collider2D>();
            //    sliders[i].SetValueWithoutNotify((col.bounciness - bounds.Item1) / bounds.Item2);
            //    inputFields[i].SetTextWithoutNotify("" + Mathf.Round(col.bounciness * 100f) / 100f);
            //}
            //else if (i == 11)
            //{
            //    (float, float) bounds = (0f, 1f);
            //    Collider2D col = focused.GetComponent<Collider2D>();
            //    sliders[i].SetValueWithoutNotify((col.friction - bounds.Item1) / bounds.Item2);
            //    inputFields[i].SetTextWithoutNotify("" + Mathf.Round(col.friction * 100f) / 100f);
            //}
        }
    }

    public void sliderValueChange (int idx)
    {
        if (sliders[idx].gameObject != EventSystem.current.currentSelectedGameObject) return;
       // Debug.Log(idx);
        float value = sliders[idx].value;
        if (idx == 0)
        {
            (float, float) bounds = (0.01f, 1000f);
            float mass = bounds.Item1 + value / 1f * (bounds.Item2 - bounds.Item1);
            rbcurr.mass = mass;
            inputFields[idx].SetTextWithoutNotify("" + Mathf.Round(mass * 100f) / 100f);
        }
        else if (idx == 1)
        {
            (float, float) bounds = (0.01f, 1000f);
            float length = bounds.Item1 + value / 1f * (bounds.Item2 - bounds.Item1);
            if (namecurr == "FixedRectangle") transcurr.localScale = new v2(length / Util.FixedRectWidthMultiplier, transcurr.localScale.y);
            else if (namecurr == "MoveableRectangle") transcurr.localScale = new v2(length / Util.MoveableRectWidthMultiplier, transcurr.localScale.y);
            inputFields[idx].SetTextWithoutNotify("" + Mathf.Round(length * 100f) / 100f);
        }
        else if (idx == 2)
        {
            (float, float) bounds = (0.01f, 1000f);
            float width = bounds.Item1 + value / 1f * (bounds.Item2 - bounds.Item1);
            if (namecurr == "FixedRectangle") transcurr.localScale = new v2(transcurr.localScale.x, width / Util.FixedRectHeightMultiplier);
            else if (namecurr == "MoveableRectangle") transcurr.localScale = new v2(transcurr.localScale.x, width / Util.MoveableRectHeightMultiplier);
            inputFields[idx].SetTextWithoutNotify("" + Mathf.Round(width * 100f) / 100f);
        }
        else if (idx == 3) //circle
        {
            (float, float) bounds = (0.01f, 100f);
            float diameter = bounds.Item1 + value / 1f * (bounds.Item2 - bounds.Item1);
            transcurr.localScale = new v2(diameter, diameter);
            inputFields[idx].SetTextWithoutNotify("" + Mathf.Round(diameter * 100f) / 100f);
        }
        else if (idx == 4) //spring constant (N/m)
        {
            (float, float) bounds = (0.01f, 1000f);
            SpringControl spring = focused.GetComponent<SpringControl>();
            float constant = bounds.Item1 + value / 1f * (bounds.Item2 - bounds.Item1);
            spring.setSpringConstant(constant);
            inputFields[idx].SetTextWithoutNotify("" + Mathf.Round(constant * 100f) / 100f);
        }
        else if (idx == 5) //force
        {
            (float, float) bounds = (0.01f, 1000f);
            float newtons = bounds.Item1 + value / 1f * (bounds.Item2 - bounds.Item1);
            ForceControl force = focused.GetComponent<ForceControl>();
            force.setForce(newtons);
            //inputFields[idx].SetTextWithoutNotify("" + Mathf.Round(newtons * 100f) / 100f);
        }
        else if (idx == 6) //velocity
        {
            (float, float) bounds = (0.01f, 100f);
            float speed = bounds.Item1 + value / 1f * (bounds.Item2 - bounds.Item1);
            VelocityControl velocity = focused.GetComponent<VelocityControl>();
            velocity.setSpeed(speed);
        }
        else if (idx == 7)
        {
            (float, float) bounds = (0f, 360f);
            float angle = bounds.Item1 + value / 1f * (bounds.Item2 - bounds.Item1);
            if (namecurr == "Force") this.focused.GetComponent<ForceControl>().setAngle(angle);
            else if (namecurr == "Velocity") this.focused.GetComponent<VelocityControl>().setAngle(angle);
            else
            {
                transcurr.localEulerAngles = new v3(0, 0, angle);
                inputFields[idx].SetTextWithoutNotify("" + Mathf.Round(angle * 100f) / 100f);
            }
        }
        else if (idx == 8) //equilibrium length
        {
            (float, float) bounds = (0.01f, 100f);
            SpringControl spring = focused.GetComponent<SpringControl>();
            float elength = bounds.Item1 + value / 1f * (bounds.Item2 - bounds.Item1);
            spring.setElength(elength);
            inputFields[idx].SetTextWithoutNotify("" + Mathf.Round(elength * 100f) / 100f);
        }
        else if (idx == 9) //break force (N)
        {
            (double, double) bounds = (0.01, 999999999);
            SpringControl spring = focused.GetComponent<SpringControl>();
            double breakforce = bounds.Item1 + value * (bounds.Item2 - bounds.Item1);
            spring.setBreakForce(breakforce);
            inputFields[idx].SetTextWithoutNotify("" + Math.Round(breakforce * 100f) / 100f);
        }
        else if (idx == 10)
        {
            (float, float) bounds = (0f, 1f);
            Collider2D col = focused.GetComponent<Collider2D>();
            float friction = bounds.Item1 + value * (bounds.Item2 - bounds.Item1);
            col.sharedMaterial.friction = friction;
            Attachable.ResetAllColliders();
            inputFields[idx].SetTextWithoutNotify("" + Mathf.Round(friction * 100f) / 100f);
        }
        //else if (idx == 10)
        //{
        //    (float, float) bounds = (0f, 1f);
        //    Collider2D col = focused.GetComponent<Collider2D>();
        //    float bounciness = bounds.Item1 + value * (bounds.Item2 - bounds.Item1);
        //    col.sharedMaterial.bounciness = bounciness;
        //    inputFields[idx].SetTextWithoutNotify("" + Mathf.Round(bounciness * 100f) / 100f);
        //}
        //else if (idx == 11)
        //{

        //}
    }

    public void fieldValueChange(int idx)
    {
        double val;
        string text = inputFields[idx].text;
        if (text == "") val = 0;
        else if (!double.TryParse(text, out val)) { return; }

        if (idx == 0)
        {
            (float, float) bounds = (0.01f, 1000f);
            float value = (float)val;
            value = Mathf.Clamp(value, bounds.Item1, bounds.Item2);
            rbcurr.mass = value;
            sliders[idx].SetValueWithoutNotify((value - bounds.Item1)/bounds.Item2);
            if ((text[text.Length-1] != '.' && text[text.Length - 1] != '0')||(text[text.Length - 1] == '0' && !text.Contains("."))) inputFields[idx].SetTextWithoutNotify("" + Mathf.Round(value * 100f) / 100f);
        }
        else if (idx == 1)
        {
            (float, float) bounds = (0.01f, 1000f);
            float value = (float)val;
            value = Mathf.Clamp(value, bounds.Item1, bounds.Item2);
            float length = value;
            if (namecurr == "FixedRectangle") transcurr.localScale = new v2(length / Util.FixedRectWidthMultiplier, transcurr.localScale.y);
            else if (namecurr == "MoveableRectangle") transcurr.localScale = new v2(length / Util.MoveableRectWidthMultiplier, transcurr.localScale.y);
            sliders[idx].SetValueWithoutNotify((length - bounds.Item1) / bounds.Item2);
            if ((text[text.Length - 1] != '.' && text[text.Length - 1] != '0') || (text[text.Length - 1] == '0' && !text.Contains("."))) inputFields[idx].SetTextWithoutNotify("" + Mathf.Round(length * 100f) / 100f);
        }
        else if (idx == 2)
        {
            (float, float) bounds = (0.01f, 1000f);
            float value = (float)val;
            value = Mathf.Clamp(value, bounds.Item1, bounds.Item2);
            float width = value;
            if (namecurr == "FixedRectangle") transcurr.localScale = new v2(transcurr.localScale.x, width / Util.FixedRectHeightMultiplier);
            else if (namecurr == "MoveableRectangle") transcurr.localScale = new v2(transcurr.localScale.x, width / Util.MoveableRectHeightMultiplier);
            sliders[idx].SetValueWithoutNotify((width - bounds.Item1)/bounds.Item2);
            if ((text[text.Length - 1] != '.' && text[text.Length - 1] != '0') || (text[text.Length - 1] == '0' && !text.Contains("."))) inputFields[idx].SetTextWithoutNotify("" + Mathf.Round(width * 100f) / 100f);
        }
        else if(idx == 3) //circle
        {
            (float, float) bounds = (0.01f, 100f);
            float value = (float)val;
            value = Mathf.Clamp(value, bounds.Item1, bounds.Item2);
            transcurr.localScale = new v2(value, value);
            sliders[idx].SetValueWithoutNotify((value - bounds.Item1)/bounds.Item2);
            if ((text[text.Length - 1] != '.' && text[text.Length - 1] != '0') || (text[text.Length - 1] == '0' && !text.Contains("."))) inputFields[idx].SetTextWithoutNotify("" + Mathf.Round(value * 100f) / 100f);
        }
        else if (idx == 4)
        {
            (float, float) bounds = (0.01f, 1000f);
            float value = (float)val;
            value = Mathf.Clamp(value, bounds.Item1, bounds.Item2);
            SpringControl spring = focused.GetComponent<SpringControl>();
            spring.setSpringConstant(value);
            sliders[idx].SetValueWithoutNotify((value - bounds.Item1) / bounds.Item2);
            if ((text[text.Length - 1] != '.' && text[text.Length - 1] != '0') || (text[text.Length - 1] == '0' && !text.Contains("."))) inputFields[idx].SetTextWithoutNotify("" + Mathf.Round(value * 100f) / 100f);
        }
        else if (idx == 5)
        {
            (float, float) bounds = (0.01f, 1000f);
            float value = (float)val;
            value = Mathf.Clamp(value, bounds.Item1, bounds.Item2);
            ForceControl force = focused.GetComponent<ForceControl>();
            if ((text[text.Length - 1] != '.' && text[text.Length - 1] != '0') || (text[text.Length - 1] == '0' && !text.Contains("."))) inputFields[idx].SetTextWithoutNotify("" + Mathf.Round(value * 100f) / 100f);
            force.setForce(value);
            //sliders[idx].SetValueWithoutNotify((value - bounds.Item1) / bounds.Item2);
            //inputFields[idx].SetTextWithoutNotify("" + Mathf.Round(value * 100f) / 100f);
        }
        else if (idx == 6)
        {
            (float, float) bounds = (0.01f, 100f);
            float value = (float)val;
            value = Mathf.Clamp(value, bounds.Item1, bounds.Item2);
            VelocityControl velocity = focused.GetComponent<VelocityControl>();
            if ((text[text.Length - 1] != '.' && text[text.Length - 1] != '0') || (text[text.Length - 1] == '0' && !text.Contains("."))) inputFields[idx].SetTextWithoutNotify("" + Mathf.Round(value * 100f) / 100f);
            velocity.setSpeed(value);
        }
        else if (idx == 7)
        {
            (float, float) bounds = (0f, 360f);
            float value = (float)val;
            value = Mathf.Clamp(value, bounds.Item1, bounds.Item2);
            if (namecurr == "Force")
            {
                if ((text[text.Length - 1] != '.' && text[text.Length - 1] != '0') || (text[text.Length - 1] == '0' && !text.Contains("."))) inputFields[idx].SetTextWithoutNotify("" + Mathf.Round(value * 100f) / 100f);
                this.focused.GetComponent<ForceControl>().setAngle(value);
            }
            else if (namecurr == "Velocity")
            {
                if ((text[text.Length - 1] != '.' && text[text.Length - 1] != '0') || (text[text.Length - 1] == '0' && !text.Contains("."))) inputFields[idx].SetTextWithoutNotify("" + Mathf.Round(value * 100f) / 100f);
                this.focused.GetComponent<VelocityControl>().setAngle(value);
            }
            else
            {
                transcurr.localEulerAngles = new v3(0, 0, value);
                sliders[idx].SetValueWithoutNotify((value - bounds.Item1) / bounds.Item2);
                if ((text[text.Length - 1] != '.' && text[text.Length - 1] != '0') || (text[text.Length - 1] == '0' && !text.Contains("."))) inputFields[idx].SetTextWithoutNotify("" + Mathf.Round(value * 100f) / 100f);
            }
        }
        else if (idx == 8) //equilibrium length
        {
            (float, float) bounds = (0.01f, 100f);
            float value = (float)val;
            value = Mathf.Clamp(value, bounds.Item1, bounds.Item2);
            SpringControl spring = focused.GetComponent<SpringControl>();
            spring.setElength(value);
            sliders[idx].SetValueWithoutNotify((value - bounds.Item1) / bounds.Item2);
            if ((text[text.Length - 1] != '.' && text[text.Length - 1] != '0') || (text[text.Length - 1] == '0' && !text.Contains("."))) inputFields[idx].SetTextWithoutNotify("" + Mathf.Round(value * 100f) / 100f);
        }
        else if (idx == 9) //break force (N)
        {
            (double, double) bounds = (0.01, 999999999);
            SpringControl spring = focused.GetComponent<SpringControl>();
            double breakforce = Math.Max(Math.Min(val, bounds.Item2), bounds.Item1);
            spring.setBreakForce(breakforce);
            sliders[idx].SetValueWithoutNotify((float)((breakforce - bounds.Item1) / bounds.Item2));
            if ((text[text.Length - 1] != '.' && text[text.Length - 1] != '0') || (text[text.Length - 1] == '0' && !text.Contains("."))) inputFields[idx].SetTextWithoutNotify("" + Math.Round(breakforce * 100f) / 100f);
        }
        else if (idx == 10)
        {
            (float, float) bounds = (0f, 1f);
            Collider2D col = focused.GetComponent<Collider2D>();
            float value = (float)val;
            value = Mathf.Clamp(value, bounds.Item1, bounds.Item2);
            col.sharedMaterial.friction = value;
            Attachable.ResetAllColliders();
            sliders[idx].SetValueWithoutNotify((value - bounds.Item1) / bounds.Item2);
            if ((text[text.Length - 1] != '.' && text[text.Length - 1] != '0') || (text[text.Length - 1] == '0' && !text.Contains("."))) inputFields[idx].SetTextWithoutNotify("" + Mathf.Round(value * 100f) / 100f);
        }
        //else if (idx == 10)
        //{
        //    (float, float) bounds = (0f, 1f);
        //    Collider2D col = focused.GetComponent<Collider2D>();
        //    float value = (float)val;
        //    value = Mathf.Clamp(value, bounds.Item1, bounds.Item2);
        //    col.sharedMaterial.bounciness = value;
        //    sliders[idx].SetValueWithoutNotify((value - bounds.Item1) / bounds.Item2);
        //    if ((text[text.Length - 1] != '.' && text[text.Length - 1] != '0') || (text[text.Length - 1] == '0' && !text.Contains("."))) inputFields[idx].SetTextWithoutNotify("" + Mathf.Round(value * 100f) / 100f);
        //}
        //else if (idx == 11)
        //{

        //}
    }

    public void fieldEndEdit(int idx)
    {

    }

    public void Delete ()
    {
        if (namecurr == "") return;
        if (focused == null) return;
        focused.GetComponent<Destructable>().Destruct();
    }
}
