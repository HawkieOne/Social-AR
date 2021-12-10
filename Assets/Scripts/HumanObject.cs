using TMPro;
using UnityEngine;

public class HumanObject : MonoBehaviour
{
    [SerializeField]
    private bool IsSelected;

    [SerializeField]
    private bool IsLocked;

    public bool Selected
    {
        get
        {
            return this.IsSelected;
        }
        set
        {
            IsSelected = value;
        }
    }

    public bool Locked
    {
        get
        {
            return this.IsLocked;
        }
        set
        {
            IsLocked = value;
        }
    }

    [SerializeField]
    private TextMeshPro nameText;

    [SerializeField]
    private Canvas canvasComponent;

    [SerializeField]
    private string OverlayDisplayText;

    public void SetOverlayText(string text)
    {
        nameText.text = text;
    }

    void Awake()
    {
        //Håkan = GetComponentInChildren<TextMeshPro>();
        if (nameText != null)
        {
            string[] names = new string[] { "Reza", "Max", "Oskar", "Håkan" };
            int number = Random.Range(0, 4);
            nameText.text = names[number];
        }
    }

    private void Update()
    {

    }

    public void ToggleOverlay()
    {
        //Håkan.gameObject.SetActive(IsSelected);
        //Håkan.text = OverlayDisplayText;
    }

    public void ToggleCanvas()
    {
        //canvasComponent?.gameObject.SetActive(IsSelected);
    }
}
