using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using DG.Tweening;

public class XPCE17_Player : MonoBehaviour
{
    public static XPCE17_Player instance;

    public SteamVR_PlayArea playArea;
    public Camera cam;

    public SteamVR_Action_Boolean inputGrab;
    public SteamVR_Input_Sources handType;

    public bool isGrab;

    private float baseFOV;

    private void Awake()
    {
        instance = this;

        baseFOV = cam.fieldOfView;
    }

    private void Start()
    {
        inputGrab.AddOnStateDownListener(OnInputDown, handType);
        inputGrab.AddOnStateUpListener(OnInputUp, handType);
    }

    private void Update()
    {
        
    }

    private void OnInputDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        isGrab = true;
    }
 
    private void OnInputUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        isGrab = false;
    }
}
