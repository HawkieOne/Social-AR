using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARRaycastManager))]
public class PlacementWithText : MonoBehaviour
{
    [SerializeField]
    private GameObject placedPrefab;

    [SerializeField]
    private GameObject welcomePanel;

    [SerializeField]
    private Button dismissButton;

    [SerializeField]
    private bool applyScalingPerObject = false;

    [SerializeField]
    private Slider scaleSlider;

    [SerializeField]
    private Text scaleTextValue;

    [SerializeField]
    private Button toggleOptionsButton;

    [SerializeField]
    private GameObject options;

    [SerializeField]
    private Camera arCamera;

    [SerializeField]
    private Text redStats, greenStats, blueStats;

    [SerializeField]
    private Button clearButton;

    [SerializeField]
    private Text statusText;

    private GameObject placedObject;

    private Vector2 touchPosition = default;

    private ARRaycastManager arRaycastManager;

    private ARSessionOrigin aRSessionOrigin;

    private bool onTouchHold = false;

    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private PlacementObject lastSelectedObject = null;

    private int count = 0;
    private int nextUpdate = 1;

    private GameObject PlacedPrefab
    {
        get
        {
            return placedPrefab;
        }
        set
        {
            placedPrefab = value;
        }
    }


    void Awake()
    {
        arRaycastManager = GetComponent<ARRaycastManager>();
        aRSessionOrigin = GetComponent<ARSessionOrigin>();
        dismissButton.onClick.AddListener(Dismiss);
        scaleSlider.onValueChanged.AddListener(ScaleChanged);
        toggleOptionsButton.onClick.AddListener(ToggleOptions);
        clearButton.onClick.AddListener(() => ClearGameObjects());
        statusText.text = "Status: <color=red>WAIT</color>";
    }

    void ClearGameObjects()
    {
        GameObject[] prefabs = GameObject.FindGameObjectsWithTag("Prototype");
        foreach (GameObject prefab in prefabs)
        {
            Destroy(prefab);
        }
    }

    private void ToggleOptions()
    {
        if (options.activeSelf)
        {
            toggleOptionsButton.GetComponentInChildren<Text>().text = "O";
            options.SetActive(false);
        }
        else
        {
            toggleOptionsButton.GetComponentInChildren<Text>().text = "X";
            options.SetActive(true);
        }
    }

    private void Dismiss() => welcomePanel.SetActive(false);
    private void ScaleChanged(float newValue)
    {
        if (applyScalingPerObject)
        {
            if (lastSelectedObject != null && lastSelectedObject.Selected)
            {
                lastSelectedObject.transform.parent.localScale = Vector3.one * newValue;
            }
        }
        else
            aRSessionOrigin.transform.localScale = Vector3.one * newValue;

        scaleTextValue.text = $"Scale {newValue}";
    }

    private void updateCount()
    {
        count++;
    }

    void Update()
    {
        // do not capture events unless the welcome panel is hidden or if UI is selected
        if (welcomePanel.activeSelf)
            return;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            //if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            //    return;

            touchPosition = touch.position;
            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = arCamera.ScreenPointToRay(touch.position);
                RaycastHit hitObject;
                if (Physics.Raycast(ray, out hitObject))
                {
                    lastSelectedObject = hitObject.transform.GetComponent<PlacementObject>();
                    if (lastSelectedObject != null)
                    {
                        blueStats.text = "INSIDE SELECT";
                        PlacementObject[] allOtherObjects = FindObjectsOfType<PlacementObject>();
                        foreach (PlacementObject placementObject in allOtherObjects)
                        {
                            if (placementObject != lastSelectedObject)
                            {
                                placementObject.Selected = false;
                            }
                            else
                            {
                                blueStats.text = "SELECTED";
                                placementObject.Selected = true;
                            }
                        }
                    }
                }                

                greenStats.text = touchPosition.ToString();
                redStats.text = hits[0].ToString();              
                if (arRaycastManager.Raycast(touchPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
                {
                    //statusText.text = $"Status: <color='red'>BEGIN</ color >";
                    Pose hitPose = hits[0].pose;

                    //lastSelectedObject = Instantiate(placedPrefab, hitPose.position, hitPose.rotation).GetComponent<PlacementObject>();
                    if (lastSelectedObject == null)
                    {
                        lastSelectedObject = Instantiate(placedPrefab, hitPose.position, hitPose.rotation).GetComponent<PlacementObject>();
                    }
                }
                else
                {
                    redStats.text = "Not inside";
                }
            }
            

            if (touch.phase == TouchPhase.Moved)
            {
                if (arRaycastManager.Raycast(touchPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = hits[0].pose;

                    if (lastSelectedObject != null && lastSelectedObject.Selected)
                    {
                        lastSelectedObject.transform.parent.position = hitPose.position;
                        lastSelectedObject.transform.parent.rotation = hitPose.rotation;
                    }
                }
            }
        }
        if (Time.time >= nextUpdate)
        {
            // Change the next update (current second+1)
            nextUpdate = Mathf.FloorToInt(Time.time) + 1;
            // Call your fonction
            updateCount();
            if (count >= 3)
            {
                statusText.text = "Status: <color=green>Place objects in area</color>";
            }
        }
    }
}
