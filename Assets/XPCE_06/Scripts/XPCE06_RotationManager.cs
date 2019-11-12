using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPCE06_RotationManager : MonoBehaviour
{
    public float RotatSpeed = 20.0f;
    public Vector3 Direction;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Direction * RotatSpeed * Time.deltaTime);

        if (Input.GetKey(KeyCode.A))
        {
            Direction = Vector3.right;
        }

        if (Input.GetKey(KeyCode.Z))
        {
            Direction = Vector3.left;
        }

        if (Input.GetKey(KeyCode.E))
        {
            Direction = Vector3.up;
        }

        if (Input.GetKey(KeyCode.R))
        {
            Direction = Vector3.down;
        }

        if (Input.GetKey(KeyCode.P))
        {
            Direction = Vector3.zero;
        }
    }
}
