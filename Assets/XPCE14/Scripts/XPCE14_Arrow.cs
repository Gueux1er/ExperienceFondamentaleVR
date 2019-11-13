using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPCE14_Arrow : MonoBehaviour
{
    public Material normalMaterial;
    public Material canGrabMaterial;
    public MeshRenderer mesh;

    public XPCE14_ControlSlab controlSlab;
    public int ID;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("XPCE14_Hand"))
        {
            mesh.material = canGrabMaterial;
            XPCE14_Player.instance.currentControlSlab = controlSlab;
            XPCE14_Player.instance.currentArrowID = ID;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("XPCE14_Hand"))
        {
            mesh.material = canGrabMaterial;
            XPCE14_Player.instance.currentControlSlab = controlSlab;
            XPCE14_Player.instance.currentArrowID = ID;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("XPCE14_Hand"))
        {
            mesh.material = normalMaterial;
            XPCE14_Player.instance.currentControlSlab = null;
            XPCE14_Player.instance.currentArrowID = -1;
        }
    }
}
