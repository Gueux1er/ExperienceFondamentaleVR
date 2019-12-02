using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

public class XPCE14_Player : MonoBehaviour
{
    public static XPCE14_Player instance;

    [Header("Control Visual Effect & Mouvement")]
    public bool fade;
    public bool blur;
    public bool reduceFOV;
    public bool darkBorder;
    public float speed = 2.5f;

    [Space(20)]

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
        Vector3 endPos = startPos + ((currentArrowID == 0) ? Vector3.forward : (currentArrowID == 1) ? Vector3.right : (currentArrowID == 2) ? Vector3.back : Vector3.left);
        float startDistance = Vector3.Distance(startPos, endPos);

        // Active next control slab
        currentControlSlab.Desable();

        // Camera effect
        cam.DOKill();
        cam.DOFieldOfView(baseFOV + 20, startDistance / (speed * 2)).SetEase(Ease.InOutSine).SetLoops(2, LoopType.Yoyo);

        if (fade)
        {
            SteamVR_Fade.View(new Color(0, 0, 0, 0.5f), startDistance / (speed * 2));
            DOVirtual.DelayedCall(startDistance / 5, () =>
            {
                SteamVR_Fade.View(new Color(0, 0, 0, 0), startDistance / (speed * 2));
            });
        }

        // Deplacement player
        playArea.transform.DOKill();
        playArea.transform.DOMove(new Vector3(endPos.x, startPos.y, endPos.z), startDistance / speed).SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                isMove = false;
                currentControlSlab.transform.position = endPos;
                currentControlSlab.Active();
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