using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using DG.Tweening;

public class XPCE14_Player : MonoBehaviour
{
    public static XPCE14_Player instance;

    public SteamVR_PlayArea playArea;
    public Camera cam;

    public SteamVR_Action_Boolean inputGrab;
    public SteamVR_Input_Sources handType;

    public bool isMove;

    public bool isGrab;
    public XPCE14_ControlSlab currentControlSlab;
    public int currentArrowID = -1;

    private float baseFOV;

    private void Awake()
    {
        instance = this;
        currentArrowID = -1;

        baseFOV = cam.fieldOfView;
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
            MovingToNextControlSlab();
        }
    }

    [ContextMenu("Debug MovementToNextControlSlab")]
    private void MovingToNextControlSlab()
    {
        Vector3 startPos = playArea.transform.position;
        Vector3 targetPos = currentControlSlab.controlers[currentArrowID].targetControlSlab.transform.position;
        XPCE14_ControlSlab nextControlSlab = currentControlSlab.controlers[currentArrowID].targetControlSlab;
        float startDistance = Vector3.Distance(startPos, targetPos);

        // Active next control slab
        nextControlSlab.Desable();
        
        // Camera effect
        cam.DOKill();
        cam.DOFieldOfView(baseFOV + 20, startDistance / 4).SetEase(Ease.InOutSine).SetLoops(2, LoopType.Yoyo);

        // Deplacement player
        playArea.transform.DOKill();
        playArea.transform.DOMove(new Vector3(targetPos.x, startPos.y, targetPos.z), startDistance / 2).SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                isMove = false;
                nextControlSlab.Active();
            });
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
