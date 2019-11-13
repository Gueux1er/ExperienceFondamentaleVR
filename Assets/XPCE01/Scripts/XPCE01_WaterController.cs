using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPCE01_WaterController : MonoBehaviour
{
    public static XPCE01_WaterController instance;

    [Header("Parameters")]
    public float minHeight = 0;
    public float maxHeight = 2;
    public float duration = 10;
    public bool isYoyo = true;

    [Space(20)]
    public GameObject waterObject;
    public AnimationCurve waterMovementCurve;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        waterObject.transform.position = Vector3.up * minHeight;
    }

    public float t;
    private void Update()
    {
        if (isYoyo)
            t = Mathf.PingPong(Time.time / duration, 1);
        else
            t += Time.deltaTime / duration;

        waterObject.transform.position = Vector3.Lerp(Vector3.up * minHeight, Vector3.up * maxHeight, waterMovementCurve.Evaluate(t));
    }
}