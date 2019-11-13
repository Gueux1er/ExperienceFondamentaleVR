using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Valve.VR;

public class XPCE17_Player : MonoBehaviour
{
    public static XPCE17_Player instance;

    public float distanceToHook = 10;

    public SteamVR_PlayArea playArea;

    public SteamVR_Action_Boolean inputGrab;
    public SteamVR_Input_Sources handType;

    public Transform handTransform;
    public Transform targetTransform;
    public RaycastHit targetHit;
    public LineRenderer line;

    public bool isInputGrab;
    public bool isHook;

    private float baseFOV;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        inputGrab.AddOnStateDownListener(OnInputDown, handType);
        inputGrab.AddOnStateUpListener(OnInputUp, handType);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(handTransform.position, handTransform.position + handTransform.forward);
    }

    private void Update()
    {
        if (isHook)
            return;

        if (Physics.Raycast(handTransform.position, handTransform.forward, out targetHit, distanceToHook, 1 << 20))
        {
            print("Hook Ready");

            targetTransform.gameObject.SetActive(true);
            targetTransform.position = targetHit.point;

            if (isInputGrab)
            {
                MovingToHook();
            }
        }
        else
        {
            targetTransform.gameObject.SetActive(false);
        }
    }

    private void MovingToHook()
    {
        isHook = true;

        targetTransform.gameObject.SetActive(false);

        line.gameObject.SetActive(true);
        line.SetPosition(0, targetHit.point);

        Vector3 startPos = playArea.transform.position;
        Vector3 targetPos = targetHit.point - handTransform.forward;
        float startDistance = Vector3.Distance(startPos, targetPos);

        // Deplacement player
        playArea.transform.DOKill();
        playArea.transform.DOMove(new Vector3(targetPos.x, startPos.y, targetPos.z), startDistance / 5f).SetEase(Ease.Linear)
            .OnUpdate(() =>
            {
                line.SetPosition(1, handTransform.position);
            })
            .OnComplete(() =>
            {
                isHook = false;
                line.gameObject.SetActive(false);
            });
    }

    private void OnInputDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        isInputGrab = true;
    }

    private void OnInputUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        isInputGrab = false;
    }
}