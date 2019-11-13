using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeVR : MonoBehaviour
{
    void Start()
    {
        GetComponent<MeshRenderer>().material.color = Color.white;
    }

    public void Update()
    {
        //GetComponent<MeshRenderer>().material.color = Color.white;
    }

    public void NewParent()
    {
        GameObject c = GameObject.FindGameObjectWithTag("CameraRig");

        c.transform.SetParent(transform);
        //c.transform.localPosition = Vector3.zero + Vector3.up * transform.localScale.y;
    }

    public void ChangeColor()
    {
        GetComponent<MeshRenderer>().material.color = Color.magenta;
    }

    public void ResetColor()
    {
        GetComponent<MeshRenderer>().material.color = Color.white;
    }

    public void GrapinColor()
    {
        GetComponent<MeshRenderer>().material.color = Color.green;
    }
}
