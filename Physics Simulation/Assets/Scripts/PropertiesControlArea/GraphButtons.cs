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
        this.image = this.checkbox.GetComponent<Image>();
        image.sprite = uncheckedBoxSprite;
    }
}
