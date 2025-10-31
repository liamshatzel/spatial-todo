using UnityEngine;
#if UNITY_INCLUDE_XR_HANDS
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;
#endif
#if UNITY_INCLUDE_ARFOUNDATION
using UnityEngine.XR.ARFoundation;
#endif

namespace PolySpatial.Samples
{
    public class PinchSpawn : MonoBehaviour
    {
        [SerializeField]
        GameObject m_RightSpawnPrefab;

        [SerializeField]
        GameObject m_LeftSpawnPrefab;

        [SerializeField]
        Transform m_PolySpatialCameraTransform;

        [SerializeField]
        Transform m_CameraTransform;

#if UNITY_INCLUDE_ARFOUNDATION
        RaycastHit m_HitInfo;
#endif

#if UNITY_INCLUDE_XR_HANDS
        XRHandSubsystem m_HandSubsystem;
        XRHandJoint m_RightIndexTipJoint;
        XRHandJoint m_RightThumbTipJoint;
        XRHandJoint m_LeftIndexTipJoint;
        XRHandJoint m_LeftThumbTipJoint;
        bool m_ActiveRightPinch;
        bool m_ActiveLeftPinch;
        float m_ScaledThreshold;

        const float k_PinchThreshold = 0.01f;

        void Start()
        {
            GetHandSubsystem();
            m_ScaledThreshold = k_PinchThreshold / m_PolySpatialCameraTransform.localScale.x;
        }

        void Update()
        {
            if (!CheckHandSubsystem())
                return;

            var updateSuccessFlags = m_HandSubsystem.TryUpdateHands(XRHandSubsystem.UpdateType.Dynamic);


            //Right hand pinch
            if ((updateSuccessFlags & XRHandSubsystem.UpdateSuccessFlags.RightHandRootPose) != 0)
            {
                // assign joint values
                m_RightIndexTipJoint = m_HandSubsystem.rightHand.GetJoint(XRHandJointID.IndexTip);
                m_RightThumbTipJoint = m_HandSubsystem.rightHand.GetJoint(XRHandJointID.ThumbTip);

                DetectPinch(m_RightIndexTipJoint, m_RightThumbTipJoint, ref m_ActiveRightPinch, true);
            }

            //Left hand pinch
            if ((updateSuccessFlags & XRHandSubsystem.UpdateSuccessFlags.LeftHandRootPose) != 0)
            {
                // assign joint values
                m_LeftIndexTipJoint = m_HandSubsystem.leftHand.GetJoint(XRHandJointID.IndexTip);
                m_LeftThumbTipJoint = m_HandSubsystem.leftHand.GetJoint(XRHandJointID.ThumbTip);

                LeftPinchSpawn(m_LeftIndexTipJoint, m_LeftThumbTipJoint, ref m_ActiveLeftPinch, false);
            }
        }

        void GetHandSubsystem()
        {
            var xrGeneralSettings = XRGeneralSettings.Instance;
            if (xrGeneralSettings == null)
            {
                Debug.LogError("XR general settings not set");
            }

            var manager = xrGeneralSettings.Manager;
            if (manager != null)
            {
                var loader = manager.activeLoader;
                if (loader != null)
                {
                    m_HandSubsystem = loader.GetLoadedSubsystem<XRHandSubsystem>();
                    if (!CheckHandSubsystem())
                        return;

                    m_HandSubsystem.Start();
                }
            }
        }

        bool CheckHandSubsystem()
        {
            if (m_HandSubsystem == null)
            {
#if !UNITY_EDITOR
                Debug.LogError("Could not find Hand Subsystem");
#endif
                enabled = false;
                return false;
            }

            return true;
        }

        void LeftPinchSpawn(XRHandJoint index, XRHandJoint thumb, ref bool activeFlag, bool right)
        {
            var spawnObject = m_LeftSpawnPrefab;

            if (index.trackingState != XRHandJointTrackingState.None &&
                thumb.trackingState != XRHandJointTrackingState.None)
            {
                Vector3 indexPOS = Vector3.zero;
                Vector3 thumbPOS = Vector3.zero;

                if (index.TryGetPose(out Pose indexPose))
                {
                    // adjust transform relative to the PolySpatial Camera transform
                    indexPOS = m_PolySpatialCameraTransform.InverseTransformPoint(indexPose.position);
                }

                if (thumb.TryGetPose(out Pose thumbPose))
                {
                    // adjust transform relative to the PolySpatial Camera adjustments
                    thumbPOS = m_PolySpatialCameraTransform.InverseTransformPoint(thumbPose.position);
                }

                var pinchDistance = Vector3.Distance(indexPOS, thumbPOS);

                if (pinchDistance <= m_ScaledThreshold)
                {
                    if (!activeFlag)
                    {
                        Instantiate(spawnObject, indexPOS, Quaternion.identity);
                        activeFlag = true;
                    }
                }
                else
                {
                    activeFlag = false;
                }
            }
        }

        void DetectPinch(XRHandJoint index, XRHandJoint thumb, ref bool activeFlag, bool right)
        {
            var spawnObject = right ? m_RightSpawnPrefab : m_LeftSpawnPrefab;

            if (index.trackingState != XRHandJointTrackingState.None &&
                thumb.trackingState != XRHandJointTrackingState.None)
            {
                Vector3 indexPOS = Vector3.zero;
                Vector3 thumbPOS = Vector3.zero;

                if (index.TryGetPose(out Pose indexPose))
                {
                    // adjust transform relative to the PolySpatial Camera transform
                    indexPOS = m_PolySpatialCameraTransform.InverseTransformPoint(indexPose.position);
                }

                if (thumb.TryGetPose(out Pose thumbPose))
                {
                    // adjust transform relative to the PolySpatial Camera adjustments
                    thumbPOS = m_PolySpatialCameraTransform.InverseTransformPoint(thumbPose.position);
                }

                var pinchDistance = Vector3.Distance(indexPOS, thumbPOS);

                if (pinchDistance <= m_ScaledThreshold)
                {
                    if (!activeFlag)
                    {
                        // Raycast to find objects on layer 6 for selection
                        int layerMask = 1 << 6; // Only layer 6
                        if (Physics.Raycast(new Ray(m_CameraTransform.position, m_CameraTransform.forward), out m_HitInfo, Mathf.Infinity, layerMask))
                        {
                            // Select the object currently being looked at
                            GameObject selectedObject = m_HitInfo.transform.gameObject;
                            
                            // Visualize the selection with debug rays
                            Debug.DrawRay(m_HitInfo.point, m_HitInfo.normal * 0.5f, Color.red, 2.0f);
                            Debug.DrawRay(m_CameraTransform.position, m_CameraTransform.forward * m_HitInfo.distance, Color.green, 2.0f);

                            Debug.Log($"Selected object: {selectedObject.name}");

                            //Move the object with the hand movement

                            //Slerp for smooth movement
                            selectedObject.transform.position = Vector3.Slerp(selectedObject.transform.position, indexPOS, Time.deltaTime * 10f);
                            selectedObject.transform.rotation = Quaternion.LookRotation(indexPOS - thumbPOS);
                            
                        }
                        activeFlag = true;
                    }
                }
                else
                {
                    activeFlag = false;
                }
            }
        }
#endif
    }
}
