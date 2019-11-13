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
    public float m_distance;

    private void Start()
    {
        m_target.position = m_lookTransform.position + m_lookTransform.forward * m_distance;
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
        Vector3 posPoint = lookTransform.forward * m_distance + positionTr.position;
        m_pathCreator.bezierPath.AddSegmentToEnd(posPoint);
      //  Debug.Log("created new point at " + posPoint);
      //  m_pathCreator.bezierPath.CalculateBoundsWithTransform(m_pathCreator.transform);
      //  m_pathCreator.TriggerPathUpdate();
    }

    void OnDrawGizmos()
    {
        Vector3 posPoint = m_lookTransform.forward * m_distance + m_lookTransform.position;
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(posPoint, 1);
    }
}
