using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class AttractRepulse : MonoBehaviour
{
    public enum ActionType
    {
        Translation, Force
    }

    [Header("Gameplay parameters")]
    public ActionType actionType = ActionType.Translation;
    public float attractRadius = 5.0f;
    public float attractionForce = 5.0f;
    public float attractionSpeed = 1.0f;
    public float repulsionRadius = 5.0f;
    public float repulsionForce = 5.0f;
    public float repulsionSpeed = 1.0f;

    [Header("Input parameters")]

    public Transform[] attractPoints;
    public Transform[] repulsePoints;

    public SteamVR_Action_Single attractInput;
    public SteamVR_Action_Single repulseInput;
    public SteamVR_Action_Boolean reverseAttractInput;
    public SteamVR_Action_Boolean reverseRepulseInput;
    public SteamVR_Input_Sources attractHand;
    public SteamVR_Input_Sources repulseHand;
    public SteamVR_Input_Sources globalHand;

    private void Start()
    {
        attractInput.AddOnAxisListener(ActiveAttract, attractHand);
        repulseInput.AddOnAxisListener(ActiveRepulse, repulseHand);
        reverseAttractInput.AddOnStateDownListener(ReverseAttract, repulseHand);
        reverseRepulseInput.AddOnStateDownListener(ReverseRepulse, attractHand); 
    }

    private void ReverseAttract(SteamVR_Action_Boolean fromInput, SteamVR_Input_Sources fromSource)
    {
        attractionForce = -attractionForce;
    }

    private void ReverseRepulse(SteamVR_Action_Boolean fromInput, SteamVR_Input_Sources fromSource)
    {
        repulsionForce = -repulsionForce;
    }

    private void ActiveAttract (SteamVR_Action_Single fromInput, SteamVR_Input_Sources fromSource, float newAxis, float newDelta)
    {
        Debug.Log("Input Attract : " + newAxis);

        Rigidbody[] rbs;
        for (int i = 0; i < repulsePoints.Length; ++i)
        {
            if (GetRbsInArea(repulsePoints[i].position, attractRadius, out rbs))
            {
                switch (actionType)
                {
                    case ActionType.Translation:
                        AddTranslation(rbs, repulsionSpeed, repulsionRadius, repulsePoints[i].position);
                        break;


                    case ActionType.Force:
                        AddExplosionForce(rbs, repulsionForce * newAxis, repulsionRadius, repulsePoints[i].position); 
                        break;
                }
            }

        }
    }

    private void ActiveRepulse (SteamVR_Action_Single fromInput, SteamVR_Input_Sources fromSource, float newAxis, float newDelta)
    {
        Debug.Log("Input repulse : " + newAxis);

        Rigidbody[] rbs;
        for (int i = 0; i < attractPoints.Length; ++i)
        {
            if (GetRbsInArea(attractPoints[i].position, attractRadius, out rbs))
            {
                switch (actionType)
                {
                    case ActionType.Translation:
                        AddTranslation(rbs, attractionSpeed, attractRadius, attractPoints[i].position);
                        break;


                    case ActionType.Force:
                        AddExplosionForce(rbs, attractionForce * newAxis, attractRadius, attractPoints[i].position);
                        break;
                }
            }

        }
    }

    private void AddExplosionForce(Rigidbody[] input, float value, float radius, Vector3 center)
    {
        if (input.Length == 0.0f)
            return;
        foreach (Rigidbody rb in input)
        {
            rb.AddExplosionForce(value, center, radius);
        }
    }

    private void AddTranslation(Rigidbody[] input, float value, float radius, Vector3 center)
    {
        if (input.Length == 0.0f)
            return;
        foreach(Rigidbody rb in input)
        {
            Vector3 dir = rb.transform.position - center;
            rb.transform.Translate(dir.normalized * value * Time.deltaTime); 
        }
    }

    private bool GetRbsInArea(Vector3 position, float radius, out Rigidbody[] result)
    {
        int layerId = LayerMask.NameToLayer("Movable");
        int layerMask = 1 << layerId;
        Collider[] cols = Physics.OverlapSphere(position, radius, layerMask);
        Debug.Log(cols.Length);
        result = new Rigidbody[0];
        if (cols.Length == 0.0f)
            return false;
        result = new Rigidbody[cols.Length];
        for (int i = 0; i < cols.Length; ++i)
        {
            result[i] = cols[i].attachedRigidbody;
        }
        Debug.Log(result.Length);
        return true;
    }
}
