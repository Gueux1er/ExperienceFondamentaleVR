using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPCE01_Player : MonoBehaviour
{
    public bool isOnWater;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            isOnWater = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            isOnWater = false;
        }
    }
}
