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
    [SerializeField] Transform m_parentHand;
    [SerializeField] GameObject m_initAsset_00;
    [SerializeField] GameObject m_initAsset_01;
    [SerializeField] GameObject m_initAsset_02;

    private void Start()
    {
        m_target.position = m_handTransform.position + m_handTransform.forward * m_distanceFocus;
    }

    private void GenerateReferencial()
    {
       //  Vector3 positionInit = Mathf.
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) || triggerAction.GetStateDown(SteamVR_Input_Sources.Any))
        {
            m_pathCreator.bezierPath.AddSegmentToEnd(GetInstanceDotPositionRay(m_handTransform, m_playerTransform));
          //  SetLastPointRotation(m_handTransform);
        }
        m_target.position = GetInstanceDotPositionRay(m_handTransform, m_playerTransform);
    }

    private Vector3 GetInstanceDotPosition(Transform handTransform, Transform playerTransform, Transform handParent)
    {
        Vector3 finalDotPosition = m_pathCreator.bezierPath.GetPoint(m_pathCreator.bezierPath.NumPoints - 1);
        Vector3 focusRay = handTransform.forward * m_distanceFocus + playerTransform.position;
        Vector3 pointToRayDirection = (focusRay - finalDotPosition).normalized;
        //Vector3 localForward = m_parentHand.InverseTransformDirection(handTransform.forward);
        Vector3 localForward = handTransform.forward - handParent.forward;
        Debug.Log("hand fwd: " + handTransform.forward + " / local fwd: " + localForward);
        Vector3 newDotPosition = finalDotPosition + localForward * m_distanceFromLastPoint;
        return newDotPosition;
    }


    private Vector3 GetInstanceDotPositionRay(Transform handTransform, Transform playerTransform)
    {
        Vector3 finalDotPosition = m_pathCreator.bezierPath.GetPoint(m_pathCreator.bezierPath.NumPoints - 1);
        Vector3 focusRay = handTransform.forward * m_distanceFocus + playerTransform.position;
        Vector3 pointToRayDirection = (focusRay - finalDotPosition).normalized;
        Vector3 newDotPosition = finalDotPosition + pointToRayDirection * m_distanceFromLastPoint;
        return newDotPosition;
    }

    private void SetLastPointRotation(Transform handTransform)
    {
        m_pathCreator.bezierPath.SetAnchorNormalAngle(m_pathCreator.bezierPath.NumAnchorPoints - 1, handTransform.eulerAngles.z);
    }
}
