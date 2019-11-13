using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using Valve.VR;

public class XPCE07_CreatePoints : MonoBehaviour
{
    public SteamVR_Action_Boolean triggerAction;

    [SerializeField] PathCreator m_pathCreator;
    [SerializeField] Transform m_lookTransform;
    [SerializeField] Transform m_playerTransform;
    [SerializeField] Transform m_target;

    [SerializeField] float m_distanceFocus;
    [SerializeField] float m_distanceFromLastPoint;


    private void Start()
    {
        m_target.position = m_lookTransform.position + m_lookTransform.forward * m_distanceFocus;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) || triggerAction.GetStateDown(SteamVR_Input_Sources.Any))
        {
            SetNewPoint(m_lookTransform, m_playerTransform);
        }
    }

    private void SetNewPoint(Transform lookTransform, Transform positionTr)
    {
        Vector3 finalDotPosition = m_pathCreator.bezierPath.GetPoint(m_pathCreator.bezierPath.NumPoints - 1);
        Vector3 focusRay = lookTransform.forward * m_distanceFocus + positionTr.position;
        Vector3 pointToRayDirection = (focusRay - finalDotPosition).normalized;
        Vector3 newDotPosition = finalDotPosition + pointToRayDirection * m_distanceFromLastPoint;
        m_pathCreator.bezierPath.AddSegmentToEnd(newDotPosition);
    }
}
