using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class AnchorPlacer : MonoBehaviour
{
    public Camera arCamera;
    public GameObject contentPrefab;  // what you want to stick to the world
    ARRaycastManager _raycast;
    ARAnchorManager _anchors;
    static List<ARRaycastHit> _hits = new();

    void Awake()
    {
        _raycast = FindAnyObjectByType<ARRaycastManager>();
        _anchors = FindAnyObjectByType<ARAnchorManager>();
    }

    async void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        // Ray from center of screen
        var p = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

        // Raycast against planes only (Mesh is not a valid TrackableType)
        if (_raycast.Raycast(p, _hits, TrackableType.PlaneWithinPolygon))
        {
            var pose = _hits[0].pose;

            // Create an anchor *attached to the trackable* for extra stability
            var trackable = _hits[0].trackable as ARTrackable;
            ARAnchor anchor = null;
            if (trackable)
            {
                var anchorMgr = _anchors;
                if (anchorMgr != null)
                    anchor = anchorMgr.AttachAnchor((ARPlane)trackable, pose);
            }
            // Fallback: standalone anchor
            if (anchor == null)
            {
                var result = await _anchors.TryAddAnchorAsync(pose);
                // AR Foundation's async results expose a status and a value.
                // Check the status for success and use the returned value.
                if (result.status.IsSuccess())
                    anchor = result.value;
            }
            if (anchor == null) return;

            // Parent your content under the anchor so it stays put
            var go = Instantiate(contentPrefab, pose.position, pose.rotation);
            go.transform.SetParent(anchor.transform, true);
        }
    }
}