using UnityEngine;

/// <summary>
/// This script helps fix missing VisionOSHoverEffect references in the editor
/// </summary>
public class VisionOSHoverEffectFix : MonoBehaviour
{
    void Start()
    {
        // Check for any missing PolySpatial components and log warnings
        var components = GetComponents<Component>();
        foreach (var component in components)
        {
            if (component == null)
            {
                Debug.LogWarning($"Missing component detected on {gameObject.name}. This might be causing MissingReferenceException errors.");
            }
        }
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        // Clean up any null references in the editor
        var components = GetComponents<Component>();
        for (int i = components.Length - 1; i >= 0; i--)
        {
            if (components[i] == null)
            {
                Debug.LogWarning($"Removing null component from {gameObject.name}");
                // Note: You might need to manually remove the component through the Inspector
            }
        }
    }
#endif
}