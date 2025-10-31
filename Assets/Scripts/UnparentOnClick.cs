using PolySpatial.Samples;
using UnityEngine;

public class UnparentOnClick : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    GameObject m_ObjectToUnparent;

    [SerializeField]
    SpatialUIButton m_Button;

    void Start()
    {
        //Get the game object that this script is attached to
        m_ObjectToUnparent = gameObject;
    }

    // Update is called once per frame
    void Update()
    {

    }
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
        //Set game object to world space (unparented) while maintaining world position
        Transform originalParent = m_ObjectToUnparent.transform.parent;
        m_ObjectToUnparent.transform.SetParent(null, true);

        Debug.Log($"Unparented {m_ObjectToUnparent.name} from parent: {(originalParent ? originalParent.name : "none")}");
        Debug.Log($"Object is now in world space. Parent is: {(m_ObjectToUnparent.transform.parent ? m_ObjectToUnparent.transform.parent.name : "null")}");
    }
}
