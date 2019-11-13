using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using Valve.VR;

public class XPCE07_CreatePoints : MonoBehaviour
{
    public SteamVR_Action_Boolean triggerAction;

    [SerializeField] PathCreator m_pathCreator;
    [SerializeField] Transform m_handTransform;
    [SerializeField] Transform m_playerTransform;
    [SerializeField] Transform m_target;

    [SerializeField] float m_distanceFocus;
    [SerializeField] float m_distanceFromLastPoint;

    
    private void Start()
    {
        m_target.position = m_handTransform.position + m_handTransform.forward * m_distanceFocus;
        print(m_target.localEulerAngles.z);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) || triggerAction.GetStateDown(SteamVR_Input_Sources.Any))
        {
            m_pathCreator.bezierPath.AddSegmentToEnd(GetInstanceDotPosition(m_handTransform, m_playerTransform));
            SetLastPointRotation(m_handTransform);
        }
        m_target.position = GetInstanceDotPosition(m_handTransform, m_playerTransform);
    }

    private Vector3 GetInstanceDotPosition(Transform handTransform, Transform playerTransform)
    {
        Vector3 finalDotPosition = m_pathCreator.bezierPath.GetPoint(m_pathCreator.bezierPath.NumPoints - 1);
        Vector3 focusRay = handTransform.forward * m_distanceFocus + playerTransform.position;
        Vector3 pointToRayDirection = (focusRay - finalDotPosition).normalized;
        //Vector3 newDotPosition = finalDotPosition + pointToRayDirection * m_distanceFromLastPoint;
        Vector3 newDotPosition = finalDotPosition + handTransform.forward * m_distanceFromLastPoint;
        return newDotPosition;
    }

    private void SetLastPointRotation(Transform handTransform)
    {
        m_pathCreator.bezierPath.SetAnchorNormalAngle(m_pathCreator.bezierPath.NumAnchorPoints - 1, handTransform.localEulerAngles.z);
    }
}
