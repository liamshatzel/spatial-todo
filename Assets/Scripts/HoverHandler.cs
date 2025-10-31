using UnityEngine;

public class HoverHandler : MonoBehaviour
{
    [SerializeField]
    private Color m_HoverColor = Color.cyan;

    [SerializeField]
    private Color m_DefaultColor = Color.white;

    private Renderer m_Renderer;
    private Color m_OriginalColor;

    void Start()
    {
        m_Renderer = GetComponent<Renderer>();
        if (m_Renderer != null)
        {
            m_OriginalColor = m_Renderer.material.color;
        }
    }

    // Called when mouse/pointer enters the object
    void OnMouseEnter()
    {
        if (m_Renderer != null)
        {
            m_Renderer.material.color = m_HoverColor;
            Debug.Log($"Hover entered on {gameObject.name}");
        }
    }

    // Called when mouse/pointer exits the object
    void OnMouseExit()
    {
        if (m_Renderer != null)
        {
            m_Renderer.material.color = m_OriginalColor;
            Debug.Log($"Hover exited on {gameObject.name}");
        }
    }

    // Called when mouse/pointer clicks the object
    void OnMouseDown()
    {
        Debug.Log($"Clicked on {gameObject.name}");
    }
}
