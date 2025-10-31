using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PolySpatial.Samples
{
    public class SpawnObjects : MonoBehaviour
    {
        [SerializeField]
        SpatialUIButton m_Button;

        [SerializeField]
        List<GameObject> m_ObjectsToSpawn;

        [SerializeField]
        Transform m_SpawnPosition;

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
            ForceSpawn();
        }

        public void ForceSpawn()
        {
            if (m_ObjectsToSpawn.Count == 0)
            {
                Debug.LogWarning("No objects to spawn! Please assign prefabs to the ObjectsToSpawn list.");
                return;
            }

            if (m_SpawnPosition == null)
            {
                Debug.LogWarning("No spawn position set! Please assign a Transform to SpawnPosition.");
                return;
            }

            var randomObject = Random.Range(0, m_ObjectsToSpawn.Count);

            // Spawn at world coordinates with 90 degree rotation
            Vector3 worldSpawnPosition = m_SpawnPosition.position;
            Quaternion rotation90 = Quaternion.Euler(0f, 90f, 0f); // 90 degrees around Y-axis

            GameObject spawnedObject = Instantiate(m_ObjectsToSpawn[randomObject], worldSpawnPosition, Quaternion.identity);

            // Set parent to null to ensure it's in world space (not affected by any parent transforms)
            spawnedObject.transform.SetParent(null);

            // Reset position and rotation after setting parent to null to ensure exact placement
            spawnedObject.transform.position = worldSpawnPosition;
            spawnedObject.transform.rotation = rotation90;

            Debug.Log($"Spawned {spawnedObject.name} at world position: {worldSpawnPosition}");
        }
    }
}
