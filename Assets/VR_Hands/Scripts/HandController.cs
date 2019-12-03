using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class HandController : MonoBehaviour
{
    [SerializeField] SteamVR_Action_Boolean padInputDownAction;
    [SerializeField] SteamVR_Input_Sources handType;
    CustomSteamVRBehaviourPose handPos;

    // Start is called before the first frame update
    void Start()
    {
        handPos = GetComponent<CustomSteamVRBehaviourPose>();

        handPos.ChangeOffset(1.5f);
        padInputDownAction.AddOnStateDownListener(padDown, handType);
    }

    private void padDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        handPos.ChangeOffset(handPos.ForwardDistance > 0 ? 0 : 1.5f);
    }

    // Update is called once per frame
    //void Update()
    //{
    //    Debug.Log(padInputDownAction.GetStateDown(rightHand.handType));
    //    if (padInputDownAction != null && padInputDownAction.GetStateDown(leftHand.handType))
    //    {
    //        leftHandPos.ChangeOffset(leftHandPos.ForwardDistance > 0 ? 0 : 1.5f);
    //    }

    //    if (padInputDownAction != null && padInputDownAction.GetStateDown(rightHand.handType))
    //    {
    //        rightHandPos.ChangeOffset(rightHandPos.ForwardDistance > 0 ? 0 : 1.5f);
    //    }
    //}
}
