using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class MoveWhereILook : MonoBehaviour
{
    public Transform headTransform;
    public float moveSpeed = 3.0f;
    public bool isTwoDimensional = true; 
    public SteamVR_Action_Boolean input;
    public SteamVR_Input_Sources handType;

    private bool move = false;

    private void Start()
    {
        input.AddOnStateDownListener(OnInputDown, handType);
        input.AddOnStateUpListener(OnInputUp, handType);
    }

    private void Update()
    {
        if (move)
        {
            Vector3 direction = Vector3.zero;
            if (isTwoDimensional)
            {
                direction = headTransform.forward;
                direction.y = 0.0f;
            }
            else
            {
                direction = headTransform.forward;
            }
            transform.Translate(direction * moveSpeed * Time.deltaTime);
        }
    }

    private void OnInputDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        move = true;
    }

    private void OnInputUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        move = false;
    }
}
