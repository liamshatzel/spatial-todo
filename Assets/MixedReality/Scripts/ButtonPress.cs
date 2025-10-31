using PolySpatial.Samples;
using UnityEngine;

public class ButtonPress : MonoBehaviour
{
    [SerializeField]
    SpatialUIButton m_Button;

    [SerializeField]
    GameObject m_ObjectToSpawn;

    [SerializeField]
    Transform m_SpawnPosition;

    void OnEnable()
    {
        if (m_Button)
        {
            m_Button.WasPressed += WasPressed;
            Debug.Log("Subscribed to button press event.");
        }
        else
        {
            Debug.LogWarning("Button reference is not set.");
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
        Debug.Log($"Button pressed: {buttonText}");
        ForceSpawn();
    }

    public void ForceSpawn()
    {
        Instantiate(m_ObjectToSpawn, m_SpawnPosition.position, Quaternion.identity);
    }
}
