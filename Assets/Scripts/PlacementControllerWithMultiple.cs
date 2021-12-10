using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARRaycastManager))]
public class PlacementControllerWithMultiple : MonoBehaviour
{

    [SerializeField]
    private Button arGreenButton;

    [SerializeField]
    private Button arRedButton;

    [SerializeField]
    private Button arBlueButton;

    [SerializeField]
    private Button dismissButton;

    [SerializeField]
    private GameObject welcomePanel;

    [SerializeField]
    private Text selectionText;

    [SerializeField]
    private Text redStats, greenStats, blueStats;

    [SerializeField]
    private Button clearButton;

    [SerializeField]
    private Camera arCamera;

    private GameObject placedPrefab;

    private PlacementObjectRed[] placedObjectsRed;
    private PlacementObjectBlue[] placedObjectsBlue;
    private PlacementObjectGreen[] placedObjectsGreen;

    private PlacementObjectRed lastSelectedObjectRed;
    private PlacementObjectBlue lastSelectedObjectBlue;
    private PlacementObjectGreen lastSelectedObjectGreen;

    //private bool onTouchHold = false;

    private ARRaycastManager arRaycastManager;

    private Vector2 touchPosition = default;

    private GameObject PlacedPrefabRed
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
    private GameObject PlacedPrefabGreen
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
    private GameObject PlacedPrefabBlue
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

        // set initial prefab
        ChangePrefabTo("ARBlue");

        if (arRedButton != null && arGreenButton != null && arBlueButton != null && 
            dismissButton != null && clearButton != null)
        {
            arGreenButton.onClick.AddListener(() => ChangePrefabTo("ARGreen"));
            arBlueButton.onClick.AddListener(() => ChangePrefabTo("ARBlue"));
            arRedButton.onClick.AddListener(() => ChangePrefabTo("ARRed"));
            clearButton.onClick.AddListener(() => ClearGameObjects());
            dismissButton.onClick.AddListener(Dismiss);
        }
    }

    private void Dismiss() => welcomePanel.SetActive(false);


    void ClearGameObjects()
    {
        GameObject[] prefabs = GameObject.FindGameObjectsWithTag("Balls");
        foreach (GameObject prefab in prefabs)
        {
            Destroy(prefab);
        }
    }

    void ChangePrefabTo(string prefabName)
    {
        GameObject loadedGameObject = Resources.Load<GameObject>($"Prefabs/{prefabName}");
        //placedPrefab = Resources.Load<GameObject>($"Prefabs/{prefabName}");

        if (loadedGameObject != null)
        {            
            switch (prefabName)
            {
                case "ARBlue":
                    selectionText.text = $"Selected: <color='blue'>{prefabName}</color>";
                    PlacedPrefabBlue = loadedGameObject;
                    break;
                case "ARRed":
                    selectionText.text = $"Selected: <color='red'>{prefabName}</color>";
                    PlacedPrefabRed = loadedGameObject;
                    break;
                case "ARGreen":                    
                    selectionText.text = $"Selected: <color='green'>{prefabName}</color>";
                    PlacedPrefabGreen = loadedGameObject;
                    break;
            }
            Debug.Log($"Game object with name {name} was loaded");
        }
        else
        {
            Debug.Log($"Unable to find a game object with name {name}");
        }
    }

    bool TryGetTouchPosition(out Touch touch)
    {
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            return true;
        }

        touch = default;

        return false;
    }

    void Update()
    {

        if ( welcomePanel.gameObject.activeSelf)
        {
            return;
        }

        if (!TryGetTouchPosition(out Touch touch))
        {
            return;
        }
        else
        {
            touchPosition = touch.position;
            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = arCamera.ScreenPointToRay(touch.position);
                RaycastHit hitObject;
                if (Physics.Raycast(ray, out hitObject))
                {
                    lastSelectedObjectRed = hitObject.transform.GetComponent<PlacementObjectRed>();
                    redStats.text = lastSelectedObjectRed.name;
                    if (lastSelectedObjectRed != null)
                    {
                        PlacementObjectRed[] allOtherObjects = FindObjectsOfType<PlacementObjectRed>();
                        foreach (PlacementObjectRed placementObject in allOtherObjects)
                        {
                            placementObject.Selected = placementObject == lastSelectedObjectRed;
                        }
                    }

                    lastSelectedObjectGreen = hitObject.transform.GetComponent<PlacementObjectGreen>();
                    greenStats.text = lastSelectedObjectGreen.name;
                    if (lastSelectedObjectGreen != null)
                    {
                        PlacementObjectGreen[] allOtherObjects = FindObjectsOfType<PlacementObjectGreen>();
                        foreach (PlacementObjectGreen placementObject in allOtherObjects)
                        {
                            placementObject.Selected = placementObject == lastSelectedObjectGreen;
                        }
                    }

                    lastSelectedObjectBlue = hitObject.transform.GetComponent<PlacementObjectBlue>();
                    blueStats.text = lastSelectedObjectBlue.name;
                    if (lastSelectedObjectGreen != null)
                    {
                        PlacementObjectBlue[] allOtherObjects = FindObjectsOfType<PlacementObjectBlue>();
                        foreach (PlacementObjectBlue placementObject in allOtherObjects)
                        {
                            placementObject.Selected = placementObject == lastSelectedObjectBlue;
                        }
                    }
                }
            }

            if (touch.phase == TouchPhase.Ended)
            {
                lastSelectedObjectRed.Selected = false;
                lastSelectedObjectBlue.Selected = false;
                lastSelectedObjectGreen.Selected = false;
            }
        }

        if (arRaycastManager.Raycast(touchPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;

            //Instantiate(placedPrefab, hitPose.position, hitPose.rotation).GetComponent<PlacementObject>();

            redStats.text = "SUCCESS";

            if (lastSelectedObjectRed == null && GameObject.FindObjectOfType<PlacementObjectRed>() == null)
            {
                lastSelectedObjectRed = Instantiate(placedPrefab, hitPose.position, hitPose.rotation).GetComponent<PlacementObjectRed>();
            }
            else
            {
                redStats.text = "ERROR";
                if (lastSelectedObjectRed.Selected)
                {
                    lastSelectedObjectRed.transform.position = hitPose.position;
                    lastSelectedObjectRed.transform.rotation = hitPose.rotation;
                }
            }

            if (lastSelectedObjectGreen == null)
            {
                lastSelectedObjectGreen = Instantiate(placedPrefab, hitPose.position, hitPose.rotation).GetComponent<PlacementObjectGreen>();
            }
            else
            {
                greenStats.text = "ERROR";
                if (lastSelectedObjectGreen.Selected)
                {
                    lastSelectedObjectGreen.transform.position = hitPose.position;
                    lastSelectedObjectGreen.transform.rotation = hitPose.rotation;
                }
            }


            if (lastSelectedObjectBlue == null)
            {
                lastSelectedObjectBlue = Instantiate(placedPrefab, hitPose.position, hitPose.rotation).GetComponent<PlacementObjectBlue>();
            }
            else
            {
                blueStats.text = "ERROR";
                if (lastSelectedObjectBlue.Selected)
                {
                    lastSelectedObjectBlue.transform.position = hitPose.position;
                    lastSelectedObjectBlue.transform.rotation = hitPose.rotation;
                }
            }

            //redStats.text = "RED";
            //blueStats.text = lastSelectedObjectBlue.name;
            //greenStats.text = "GREEN";
        }
    }


    static List<ARRaycastHit> hits = new List<ARRaycastHit>();
}
