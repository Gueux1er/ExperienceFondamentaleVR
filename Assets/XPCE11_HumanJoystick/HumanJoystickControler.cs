using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanJoystickControler : MonoBehaviour
{
    public Transform headTransform;
    public float deadzoneRadius = 0.5f;
    public float moveSpeed;
    public bool useProgressiveSpeed = true;
    public Vector2 progressiveMoveSpeed;
    public float maxRadius = 1.0f;

    private void Update()
    {
        Vector3 dir = headTransform.localPosition;
        dir.y = 0.0f;
        if (dir.magnitude > deadzoneRadius)
        {
            if (!useProgressiveSpeed)
            {
                transform.Translate(dir.normalized * moveSpeed * Time.deltaTime);
            }
            else
            {
                float speed = Mathf.Lerp(progressiveMoveSpeed.x, progressiveMoveSpeed.y, (dir.magnitude - deadzoneRadius) / maxRadius);
                transform.Translate(dir.normalized * speed * Time.deltaTime);
            }
        }
    }

}
