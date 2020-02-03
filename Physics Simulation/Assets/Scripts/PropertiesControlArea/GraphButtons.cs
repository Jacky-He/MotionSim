using UnityEngine;
using System.Collections;
using go = UnityEngine.GameObject;
using UnityEngine.UI;

public class GraphButtons : MonoBehaviour
{
    public bool selected = false;

    [SerializeField] private Sprite checkedBoxSprite;
    [SerializeField] private Sprite uncheckedBoxSprite;

    private go checkbox;
    private Image image;
    private Text text;
    private string objectname;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSelected(bool selected)
    {
        this.selected = selected;
        if (this.selected) image.sprite = checkedBoxSprite;
        else image.sprite = uncheckedBoxSprite;
    }

    private void Awake()
    {
        this.checkbox = this.gameObject.GetComponent<RectTransform>().Find("Checkbox").gameObject;
        this.text = this.gameObject.GetComponent<RectTransform>().Find("Text").gameObject.GetComponent<Text>();
        this.image = this.checkbox.GetComponent<Image>();
        image.sprite = uncheckedBoxSprite;
        this.objectname = this.gameObject.GetComponent<Name>().objectname;
        //if (objectname == "AccelerationX") text.color = Util.graphColors[0];
        //else if (objectname == "AccelerationY") text.color = Util.graphColors[1];
        //else if (objectname == "VelocityX") text.color = Util.graphColors[2];
        //else if (objectname == "VelocityY") text.color = Util.graphColors[3];
        //else if (objectname == "DisplacementX") text.color = Util.graphColors[4];
        //else if (objectname == "DisplacementY") text.color = Util.graphColors[5];
    }
}
