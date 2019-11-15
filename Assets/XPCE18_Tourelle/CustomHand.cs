using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class CustomHand : Hand
{
    [SerializeField] Vector3 customOffset;
    
    protected override void HandFollowUpdate()
    {
        GameObject attachedObject = currentAttachedObject;
        if (attachedObject != null)
        {
            if (currentAttachedObjectInfo.Value.interactable != null)
            {
                SteamVR_Skeleton_PoseSnapshot pose = null;

                if (currentAttachedObjectInfo.Value.interactable.skeletonPoser != null && HasSkeleton())
                {
                    pose = currentAttachedObjectInfo.Value.interactable.skeletonPoser.GetBlendedPose(skeleton);
                }

                if (currentAttachedObjectInfo.Value.interactable.handFollowTransform)
                {
                    Quaternion targetHandRotation;
                    Vector3 targetHandPosition;                        

                    if (pose == null)
                    {
                        Quaternion offset = Quaternion.Inverse(this.transform.rotation) * currentAttachedObjectInfo.Value.handAttachmentPointTransform.rotation;
                        targetHandRotation = currentAttachedObjectInfo.Value.interactable.transform.rotation * Quaternion.Inverse(offset);

                        Vector3 worldOffset = (this.transform.position - currentAttachedObjectInfo.Value.handAttachmentPointTransform.position);
                        Quaternion rotationDiff = mainRenderModel.GetHandRotation() * Quaternion.Inverse(this.transform.rotation);
                        Vector3 localOffset = rotationDiff * worldOffset;
                        targetHandPosition = currentAttachedObjectInfo.Value.interactable.transform.position + localOffset + customOffset;
                    }
                    else
                    {
                        Transform objectT = currentAttachedObjectInfo.Value.attachedObject.transform;
                        Vector3 oldItemPos = objectT.position;
                        Quaternion oldItemRot = objectT.transform.rotation;
                        objectT.position = TargetItemPosition(currentAttachedObjectInfo.Value);
                        objectT.rotation = TargetItemRotation(currentAttachedObjectInfo.Value);
                        Vector3 localSkelePos = objectT.InverseTransformPoint(transform.position);
                        Quaternion localSkeleRot = Quaternion.Inverse(objectT.rotation) * transform.rotation;
                        objectT.position = oldItemPos;
                        objectT.rotation = oldItemRot;

                        targetHandPosition = objectT.TransformPoint(localSkelePos) + customOffset;
                        targetHandRotation = objectT.rotation * localSkeleRot;
                    }

                    if (mainRenderModel != null)
                        mainRenderModel.SetHandRotation(targetHandRotation);
                    if (hoverhighlightRenderModel != null)
                        hoverhighlightRenderModel.SetHandRotation(targetHandRotation);

                    if (mainRenderModel != null)
                        mainRenderModel.SetHandPosition(targetHandPosition);
                    if (hoverhighlightRenderModel != null)
                        hoverhighlightRenderModel.SetHandPosition(targetHandPosition);
                }
            }
        }
    }

     protected override void FixedUpdate()
        {
             if (currentAttachedObject != null)
            {
                AttachedObject attachedInfo = currentAttachedObjectInfo.Value;
                if (attachedInfo.attachedObject != null)
                {
                    if (attachedInfo.HasAttachFlag(AttachmentFlags.VelocityMovement))
                    {
                        if (attachedInfo.interactable.attachEaseIn == false || attachedInfo.interactable.snapAttachEaseInCompleted)
                            UpdateAttachedVelocity(attachedInfo);

                        /*if (attachedInfo.interactable.handFollowTransformPosition)
                        {
                            skeleton.transform.position = TargetSkeletonPosition(attachedInfo);
                            skeleton.transform.rotation = attachedInfo.attachedObject.transform.rotation * attachedInfo.skeletonLockRotation;
                        }*/
                    }
                    else
                    {
                        if (attachedInfo.HasAttachFlag(AttachmentFlags.ParentToHand))
                        {
                            attachedInfo.attachedObject.transform.position = TargetItemPosition(attachedInfo) + customOffset;
                            attachedInfo.attachedObject.transform.rotation = TargetItemRotation(attachedInfo);
                        }
                    }


                    if (attachedInfo.interactable.attachEaseIn)
                    {
                        float t = Util.RemapNumberClamped(Time.time, attachedInfo.attachTime, attachedInfo.attachTime + attachedInfo.interactable.snapAttachEaseInTime, 0.0f, 1.0f);
                        if (t < 1.0f)
                        {
                            if (attachedInfo.HasAttachFlag(AttachmentFlags.VelocityMovement))
                            {
                                attachedInfo.attachedRigidbody.velocity = Vector3.zero;
                                attachedInfo.attachedRigidbody.angularVelocity = Vector3.zero;
                            }
                            t = attachedInfo.interactable.snapAttachEaseInCurve.Evaluate(t);
                            attachedInfo.attachedObject.transform.position = Vector3.Lerp(attachedInfo.easeSourcePosition, TargetItemPosition(attachedInfo) + customOffset, t);
                            attachedInfo.attachedObject.transform.rotation = Quaternion.Lerp(attachedInfo.easeSourceRotation, TargetItemRotation(attachedInfo), t);
                        }
                        else if (!attachedInfo.interactable.snapAttachEaseInCompleted)
                        {
                            attachedInfo.interactable.gameObject.SendMessage("OnThrowableAttachEaseInCompleted", this, SendMessageOptions.DontRequireReceiver);
                            attachedInfo.interactable.snapAttachEaseInCompleted = true;
                        }
                    }
                }
             }
        }
}
