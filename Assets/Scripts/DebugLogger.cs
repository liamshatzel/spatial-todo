using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class DebugLogger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log($"PlaneManagers: {FindObjectsByType<ARPlaneManager>(FindObjectsSortMode.None).Length}");
        Debug.Log($"ARSessions: {FindObjectsByType<ARSession>(FindObjectsSortMode.None).Length}");
        Debug.Log($"XROrigins: {FindObjectsByType<Unity.XR.CoreUtils.XROrigin>(FindObjectsSortMode.None).Length}");
    }
}
