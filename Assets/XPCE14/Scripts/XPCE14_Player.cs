using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class XPCE14_Player : MonoBehaviour
{
    public static XPCE14_Player instance;

    public SteamVR_Action_Boolean inputGrab;
    public SteamVR_Input_Sources handType;

    public bool isMove;

    public bool isGrab;
    public XPCE14_ControlSlab currentControlSlab;
    public int currentArrowID = -1;

    private void Awake()
    {
        instance = this;
        currentArrowID = -1;
    }

    private void Start()
    {
        inputGrab.AddOnStateDownListener(OnInputDown, handType);
        inputGrab.AddOnStateUpListener(OnInputUp, handType);

        currentControlSlab.Active();
    }

    private void Update()
    {
        if (isMove)
            return;

        if (isGrab && currentControlSlab != null && currentArrowID != -1)
        {
            isMove = true;
            StartCoroutine(MovingCoco());
        }
    }

    private IEnumerator MovingCoco()
    {
        Vector3 targetPos = currentControlSlab.controlers[currentArrowID].targetControlSlab.transform.position;
        XPCE14_ControlSlab nextControlSlab = currentControlSlab.controlers[currentArrowID].targetControlSlab;

        // Active next control slab
        nextControlSlab.Desable();
        
        // Deplacement player
        while (true)
        {

            yield return null;
        }

        // Active next control slab
        nextControlSlab.Active();
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
