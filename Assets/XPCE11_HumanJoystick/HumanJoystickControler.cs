using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanJoystickControler : MonoBehaviour
{
    public Transform headTransform;
    public float deadzoneRadius = 0.5f;
    public float moveSpeed;

    private void Update()
    {
        if (headTransform.localPosition.magnitude > deadzoneRadius)
        {
            transform.Translate(headTransform.localPosition.normalized * moveSpeed * Time.deltaTime);
        }
    }

}
