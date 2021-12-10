using TMPro;
using UnityEngine;

public class PlacementObject : MonoBehaviour
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
    private TextMeshPro H�kan, Ludvig, Per, H�kan1, Ludvig1, Per1, TimeText, TimeText2;

    [SerializeField]
    private Canvas canvasComponent;

    [SerializeField]
    private string OverlayDisplayText;

    private int count = 30;
    private int nextUpdate = 1;
    private int H�kanCount = 0;
    private int LudvigCount = 0;
    private int PerCount = 0;

    public void SetOverlayText(string text)
    {
        if (H�kan != null)
        {
            H�kan.gameObject.SetActive(false);
            H�kan.text = text;
        }
    }

    void Awake()
    {
        //H�kan = GetComponentInChildren<TextMeshPro>();
        if (H�kan != null)
        {
            //OverlayText.gameObject.SetActive(true);
            count = 30;
            //OverlayText.text = count.ToString();
        }
    }

    private void Update()
    {
        //if (OverlayText != null)
        //{
        //    count++;
        //    OverlayText.text = count.ToString();
        //}
        // If the next update is reached
        if (H�kan != null && Time.time >= nextUpdate)
        {
            Debug.Log(Time.time + ">=" + nextUpdate);
            // Change the next update (current second+1)
            nextUpdate = Mathf.FloorToInt(Time.time) + 1;
            // Call your fonction
            if (count > 0)
            {
                UpdateEverySecond();
                count--;
            }
        }
    }

    void UpdateEverySecond()
    {
        TimeText.text = count.ToString();
        TimeText2.text = count.ToString();
        if (Random.Range(1, 3) == 2)
        {
            H�kanCount++;
            H�kan.text = "H�kan: " + H�kanCount.ToString();
            H�kan1.text = "H�kan: " + H�kanCount.ToString();
        }

        if (Random.Range(1, 4) == 2)
        {
            LudvigCount++;
            Ludvig.text = "Ludvig: " + LudvigCount.ToString();
            Ludvig1.text = "Ludvig: " + LudvigCount.ToString();
        }

        if (Random.Range(1, 4) == 2)
        {
            PerCount++;
            Per.text = "Per:      : " + PerCount.ToString();
            Per1.text = "Per:     : " + PerCount.ToString();
        }

    }

    public void ToggleOverlay()
    {
        //H�kan.gameObject.SetActive(IsSelected);
        //H�kan.text = OverlayDisplayText;
    }

    public void ToggleCanvas()
    {
        //canvasComponent?.gameObject.SetActive(IsSelected);
    }
}
