using System.Collections.Generic;
using System.Net.WebSockets;
using Mono.Cecil.Cil;
using PolySpatial.Samples;
using UnityEngine;
using UnityEngine.UIElements;

public class CheckButton : MonoBehaviour
{
    [SerializeField]
    SpatialUIButton m_Button;

    [SerializeField]
    GameObject m_meshObject;

    [SerializeField]
    GameObject m_curObject;

    // Store original transforms for both objects
    Vector3 m_meshObjectOriginalScale;
    Quaternion m_meshObjectOriginalRotation;
    Vector3 m_curObjectOriginalScale;
    Quaternion m_curObjectOriginalRotation;

    bool m_IsNewMeshActive = false;
    bool m_TransformsInitialized = false;

    void OnEnable()
    {
        if (m_Button)
        {
            m_Button.WasPressed += WasPressed;
        }
    }

    void OnDisable()
    {
        if (m_Button)
        {
            m_Button.WasPressed -= WasPressed;
        }
    }

    void WasPressed(string buttonText, MeshRenderer meshrenderer)
    {
        // Initialize original transforms on first call
        if (!m_TransformsInitialized)
        {
            m_meshObjectOriginalScale = m_meshObject.transform.localScale;
            m_meshObjectOriginalRotation = m_meshObject.transform.localRotation;
            m_curObjectOriginalScale = m_curObject.transform.localScale;
            m_curObjectOriginalRotation = m_curObject.transform.localRotation;
            m_TransformsInitialized = true;
        }

        // Get the meshes from both objects
        MeshFilter meshFilter1 = m_meshObject.GetComponent<MeshFilter>();
        MeshFilter meshFilter2 = m_curObject.GetComponent<MeshFilter>();

        // Swap the meshes
        Mesh tempMesh = meshFilter1.mesh;
        meshFilter1.mesh = meshFilter2.mesh;
        meshFilter2.mesh = tempMesh;

        if (!m_IsNewMeshActive)
        {
            // First swap: meshObject now has curObject's mesh, so it should use curObject's original transform
            // curObject now has meshObject's mesh, so it should use meshObject's original transform
            ApplyParentTransformWithOriginal(m_meshObject, m_curObjectOriginalScale, m_curObjectOriginalRotation);
            ApplyParentTransformWithOriginal(m_curObject, m_meshObjectOriginalScale, m_meshObjectOriginalRotation);

            // Rotate curObject by 44 degrees on first swap
            m_curObject.transform.Rotate(0, 0, 90);

            // Translate by 0.01 in the X-axis
            m_curObject.transform.Translate(0.01f, 0, 0);

            m_IsNewMeshActive = true;
        }
        else
        {
            // Second swap: restore original mesh-transform pairings
            // meshObject gets its original mesh and transform back
            // curObject gets its original mesh and transform back
            ApplyParentTransformWithOriginal(m_meshObject, m_meshObjectOriginalScale, m_meshObjectOriginalRotation);
            ApplyParentTransformWithOriginal(m_curObject, m_curObjectOriginalScale, m_curObjectOriginalRotation);

            m_IsNewMeshActive = false;
        }
    }

    void ApplyParentTransformWithOriginal(GameObject obj, Vector3 originalScale, Quaternion originalRotation)
    {
        // Preserve original scale and rotation relative to parent
        obj.transform.localScale = originalScale;
        obj.transform.localRotation = originalRotation;

        // If you want to match the parent's world transform, uncomment these: (??)
        //obj.transform.rotation = transform.rotation * originalRotation;
        //obj.transform.localScale = Vector3.Scale(originalScale, transform.lossyScale);
    }
}
