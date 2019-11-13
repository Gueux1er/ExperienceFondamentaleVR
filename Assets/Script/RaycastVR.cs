using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastVR : MonoBehaviour
{
    LineRenderer lineRenderer;

    public GameObject cube;

    private CubeVR cubeVr;
    private bool grapin = false;
    private float size = 5;
    public GameObject cylinder;
    public GameObject sphere;

    void Start()
    {
        if (cube!= null)
            cube.GetComponent<CubeVR>().NewParent();
    }
    
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.Two)) //B   increase size
        {
            size += 0.5f;
        }

        if (OVRInput.GetDown(OVRInput.Button.One)) //A   reduce size
        {            
            if (size > 0.5f)
                size -= 0.5f;
        }

        cylinder.transform.localPosition = new Vector3(0, 0, size);
        cylinder.transform.localScale = new Vector3(cylinder.transform.localScale.x, size, cylinder.transform.localScale.z);
        sphere.transform.localPosition = new Vector3(0, 0, size*2);


        if (grapin == true && OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) != 1)
        {
            if (GameObject.FindGameObjectWithTag("CameraRig").transform.parent != null)
            {
                GameObject.FindGameObjectWithTag("CameraRig").transform.parent = null;
                cubeVr.ResetColor();
                grapin = false;
            }
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, size*2) && !grapin)
        {
            CubeVR c = hit.collider.gameObject.GetComponent<CubeVR>();

            if (c != null && cubeVr != c)
            {
                c.ResetColor();
            }

            if (c != null)
            {
                c.ChangeColor();
                cubeVr = c;
                if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) == 1)// || OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) == 1)
                {
                    c.NewParent();
                    grapin = true;
                }
            }    
        }
        else if (grapin)
        {
            cubeVr.GrapinColor();
        }
        else
        {
            if(cubeVr != null)
            {
                cubeVr.ResetColor();
                cubeVr = null;
            }
        }
    }
}
