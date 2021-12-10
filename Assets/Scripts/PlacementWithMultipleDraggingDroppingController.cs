using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARRaycastManager))]
public class PlacementWithMultipleDraggingDroppingController : MonoBehaviour
{
    [SerializeField]
    private GameObject placedPrefab;

    [SerializeField]
    private Button clearButton;

    [SerializeField]
    private GameObject welcomePanel;

    [SerializeField]
    private Button dismissButton;

    [SerializeField]
    private Camera arCamera;

    private PlacementObject[] placedObjects;

    private Vector2 touchPosition = default;

    private ARRaycastManager arRaycastManager;

    private bool onTouchHold = false;

    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private PlacementObject lastSelectedObject;

    [SerializeField]
    private Button redButton, greenButton, blueButton;

    [SerializeField]
    private Text redStats, greenStats, blueStats;

    [SerializeField]
    private Text selectionText;

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
        dismissButton.onClick.AddListener(Dismiss);

        ChangePrefabTo("ARRed");

        if (redButton != null && greenButton != null && blueButton != null)
        {
            redButton.onClick.AddListener(() => ChangePrefabTo("ARRed"));
            greenButton.onClick.AddListener(() => ChangePrefabTo("ARGreen"));
            blueButton.onClick.AddListener(() => ChangePrefabTo("ARBlue"));
            clearButton.onClick.AddListener(() => ClearGameObjects());
        }
    }

    void ClearGameObjects()
    {
        GameObject[] prefabs = GameObject.FindGameObjectsWithTag("Balls");
        foreach (GameObject prefab in prefabs)
        {
            Destroy(prefab);
        }
    }

    private void ChangePrefabTo(string name)
    {

        GameObject loadedGameObject = Resources.Load<GameObject>($"Prefabs/{name}");
        //placedPrefab = Resources.Load<GameObject>($"Prefabs/{prefabName}");

        if (loadedGameObject != null)
        {
            switch (name)
            {
                case "ARBlue":
                    selectionText.text = $"Selected: <color='blue'>{name}</color>";
                    PlacedPrefab = loadedGameObject;
                    break;
                case "ARRed":
                    selectionText.text = $"Selected: <color='red'>{name}</color>";
                    PlacedPrefab = loadedGameObject;
                    break;
                case "ARGreen":
                    selectionText.text = $"Selected: <color='green'>{name}</color>";
                    PlacedPrefab = loadedGameObject;
                    break;
            }
            Debug.Log($"Game object with name {name} was loaded");
        }
        else
        {
            Debug.Log($"Unable to find a game object with name {name}");
        }
    }

    private void Dismiss() => welcomePanel.SetActive(false);

    void Update()
    {
        // do not capture events unless the welcome panel is hidden
        if (welcomePanel.activeSelf)
            return;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

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
                        PlacementObject[] allOtherObjects = FindObjectsOfType<PlacementObject>();
                        foreach (PlacementObject placementObject in allOtherObjects)
                        {
                            placementObject.Selected = placementObject == lastSelectedObject;
                        }
                    }
                }
            }

            if (touch.phase == TouchPhase.Ended)
            {
                lastSelectedObject.Selected = false;
            }
        }

        if (arRaycastManager.Raycast(touchPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;
            greenStats.text = hits[0].pose.ToString();
            redStats.text = "PLACE";
            if (lastSelectedObject == null)
            {
                lastSelectedObject = Instantiate(placedPrefab, touchPosition, hitPose.rotation).GetComponent<PlacementObject>();
            }
            else
            {
                if (lastSelectedObject.Selected)
                {
                    lastSelectedObject.transform.position = hitPose.position;
                    lastSelectedObject.transform.rotation = hitPose.rotation;
                }
            }
        }
    }
}
