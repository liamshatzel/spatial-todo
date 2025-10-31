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
    public class ShowHideUI : MonoBehaviour
    {
        [SerializeField]
        Transform m_PolySpatialCameraTransform;

        [SerializeField]
        GameObject toShowHide;

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


            // Check right hand pinch (to prevent left hand trigger if right hand is pinching)
            bool rightHandPinching = false;
            if ((updateSuccessFlags & XRHandSubsystem.UpdateSuccessFlags.RightHandRootPose) != 0)
            {
                m_RightIndexTipJoint = m_HandSubsystem.rightHand.GetJoint(XRHandJointID.IndexTip);
                m_RightThumbTipJoint = m_HandSubsystem.rightHand.GetJoint(XRHandJointID.ThumbTip);

                rightHandPinching = IsHandPinching(m_RightIndexTipJoint, m_RightThumbTipJoint);
            }

            //Left hand pinch - only trigger if right hand is NOT pinching
            if ((updateSuccessFlags & XRHandSubsystem.UpdateSuccessFlags.LeftHandRootPose) != 0 && !rightHandPinching)
            {
                Debug.Log("Left hand detected - processing pinch for Show/Hide UI.");
                // assign joint values
                m_LeftIndexTipJoint = m_HandSubsystem.leftHand.GetJoint(XRHandJointID.IndexTip);
                m_LeftThumbTipJoint = m_HandSubsystem.leftHand.GetJoint(XRHandJointID.ThumbTip);

                LeftPinchShowHide(m_LeftIndexTipJoint, m_LeftThumbTipJoint, ref m_ActiveLeftPinch, false);
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

        void LeftPinchShowHide(XRHandJoint index, XRHandJoint thumb, ref bool activeFlag, bool right)
        {
            Debug.Log("Left pinch detected - toggling UI visibility.");
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
                        if (toShowHide.activeSelf)
                        {
                            toShowHide.SetActive(false);
                        }
                        else
                        {
                            toShowHide.SetActive(true);
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

        bool IsHandPinching(XRHandJoint index, XRHandJoint thumb)
        {
            if (index.trackingState != XRHandJointTrackingState.None &&
                thumb.trackingState != XRHandJointTrackingState.None)
            {
                Vector3 indexPOS = Vector3.zero;
                Vector3 thumbPOS = Vector3.zero;

                if (index.TryGetPose(out Pose indexPose))
                {
                    indexPOS = m_PolySpatialCameraTransform.InverseTransformPoint(indexPose.position);
                }

                if (thumb.TryGetPose(out Pose thumbPose))
                {
                    thumbPOS = m_PolySpatialCameraTransform.InverseTransformPoint(thumbPose.position);
                }

                var pinchDistance = Vector3.Distance(indexPOS, thumbPOS);
                return pinchDistance <= m_ScaledThreshold;
            }

            return false;
        }
#endif
    }
}
