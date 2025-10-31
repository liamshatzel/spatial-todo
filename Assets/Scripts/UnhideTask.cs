using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PolySpatial.Samples
{
    public class UnhideTask : MonoBehaviour
    {
        [SerializeField]
        SpatialUIButton m_Button;

        [SerializeField]
        GameObject m_ObjectsToUnhide;

        //[SerializeField]
        //Transform m_SpawnPosition;

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
            //Set game object to active
            m_ObjectsToUnhide.SetActive(true);
        }
    }
}
