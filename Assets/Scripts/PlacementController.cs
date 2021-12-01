using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class PlacementController : MonoBehaviour
{

    [SerializeField]
    private GameObject placedPrefab;
    private Color activeColor = Color.red;
    private Color inactiveColor = Color.gray;
    private Camera arCamera;
    private Touch touch = default;

    public GameObject PlacedPrefab
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


    private ARRaycastManager arRaycastManager;
    void Awake()
    {
        arRaycastManager = GetComponent<ARRaycastManager>();
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

    void SelectPlacedObject(Touch touch)
    {
        if (touch.phase == TouchPhase.Began)
        {
            Ray ray = arCamera.ScreenPointToRay(touch.position);
            RaycastHit hitObject;
            if (Physics.Raycast(ray, out hitObject)) {
                PlacementObject placementObject = hitObject.transform.GetComponent<PlacementObject>();
                if (placementObject != null)
                {
                    ChangeSelectedObject(placementObject);
                }
            }
        }
    }

    void ChangeSelectedObject(PlacementObject selected)
    {
        MeshRenderer meshRenderer = selected.GetComponent<MeshRenderer>();
        if (selected.IsSelected)
        {
            selected.IsSelected = false;
            meshRenderer.material.color = inactiveColor;
        } else
        {
            selected.IsSelected = true;
            meshRenderer.material.color = activeColor;
        }
    }

    void Update()
    {
        if (!TryGetTouchPosition(out Touch touch))
            return;

        SelectPlacedObject(touch);

        if (arRaycastManager.Raycast(touch.position, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;
            Instantiate(placedPrefab, hitPose.position, hitPose.rotation);
        }
    }


    static List<ARRaycastHit> hits = new List<ARRaycastHit>();
}
