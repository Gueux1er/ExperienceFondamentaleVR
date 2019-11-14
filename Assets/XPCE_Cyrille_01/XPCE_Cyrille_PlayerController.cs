using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class XPCE_Cyrille_PlayerController : MonoBehaviour
{
    public Transform viewTransform;
    
public SteamVR_Input_Sources LeftInputSource = SteamVR_Input_Sources.LeftHand;
public SteamVR_Input_Sources RightInputSource = SteamVR_Input_Sources.RightHand;

    public float JetSpeed = 3.0f;
    public float JetRotateSpeed = 90.0f;
    public float JetPower = 10;
    Vector3 PosJoueur;

    bool IsPressed;

    // Start is called before the first frame update
    void Start()
    {
        IsPressed = false;
    }

    public float leftTrig;
    public float rightTrig;
    void Update()
    {
        //float rightTrig = Input.GetAxis("RightTrigger");
        //float leftTrig = Input.GetAxis("LeftTrigger");
        float movejoystickLeft = 0;// = Input.GetAxis("LeftMoveJoystick");
        float movejoystickRight = 0;// = Input.GetAxis("RightMoveJoystick");
        //print("stick " + movejoystickLeft);

        leftTrig   = SteamVR_Actions._default.Squeeze.GetAxis(LeftInputSource);
        rightTrig   = SteamVR_Actions._default.Squeeze.GetAxis(RightInputSource);
        //leftTrig   = Input.GetAxis("GrabLeft");
        //leftTrig = Axis.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.LTouch);//Input.GetAxis("Oculus_CrossPlatform_PrimaryIndexTrigger");
        Debug.Log(leftTrig);
        //rightTrig   = Input.GetAxis("GrabRight");

        /*
        {
            if (rightTrig > 0)
            {
                GetComponent<Rigidbody>().velocity = Vector3.zero;
            }

            transform.Translate(Vector3.up * JetSpeed * Time.deltaTime * rightTrig);
            transform.Translate(Vector3.forward * JetSpeed * Time.deltaTime * leftTrig);
        }
        */

        Vector3 viewDirection = Vector3.ProjectOnPlane( viewTransform.forward, Vector3.up);
        Debug.Log("view : " + viewDirection);
        GetComponent<Rigidbody>().AddForce(viewDirection * JetPower * leftTrig);
        GetComponent<Rigidbody>().AddForce(Vector3.up * JetPower * leftTrig);
        GetComponent<Rigidbody>().AddForce(-viewDirection * JetPower * rightTrig);
        GetComponent<Rigidbody>().AddForce(Vector3.up * JetPower * rightTrig);

        Vector3 rotValue = Vector3.up * JetRotateSpeed * movejoystickLeft * Time.deltaTime;
        transform.Rotate(rotValue);
        
        //transform.Translate(Vector3.right * JetSpeed * Time.deltaTime * movejoystickRight);
    }
}
