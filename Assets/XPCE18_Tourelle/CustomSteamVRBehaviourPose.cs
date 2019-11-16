//======= Copyright (c) Valve Corporation, All rights reserved. ===============

using DG.Tweening;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;


/// <summary>
/// This component simplifies the use of Pose actions. Adding it to a gameobject will auto set that transform's position and rotation every update to match the pose.
/// Advanced velocity estimation is handled through a buffer of the last 30 updates.
/// </summary>
public class CustomSteamVRBehaviourPose : SteamVR_Behaviour_Pose
{
    [SerializeField] bool showForward = true;
    [SerializeField] float forwardDistance;
    public float ForwardDistance { get { return forwardDistance; } }
    [SerializeField] Vector3 customOffset;
    public Vector3 CustomOffset { get { return customOffset; } }

    [SerializeField] float offsetTweenDuration = 0.5f;

    protected override void UpdateTransform()
    {
        CheckDeviceIndex();

        var offset = showForward ? transform.forward * forwardDistance : customOffset;
        if (origin != null)
        {
            transform.position = origin.transform.TransformPoint(poseAction[inputSource].localPosition) + offset;
            transform.rotation = origin.rotation * poseAction[inputSource].localRotation;
        }
        else
        {
            transform.localPosition = poseAction[inputSource].localPosition + transform.TransformVector(offset);
            transform.localRotation = poseAction[inputSource].localRotation;
        }
    }

    public void SetShowForward(bool b)
    {
        showForward = b;
    }

    public void ChangeOffset(float distance, bool smoothChange = true)
    {
        if (smoothChange)
            DOTween.To(x => forwardDistance = x, 0, distance, offsetTweenDuration);
        else
            forwardDistance = distance;
    }

    public void ChangeOffset(Vector3 offset, bool smoothChange = true)
    {
        if (smoothChange)
            DOTween.To(() => customOffset, x => customOffset = x, offset, offsetTweenDuration);
        else
            customOffset = offset;
    }
}