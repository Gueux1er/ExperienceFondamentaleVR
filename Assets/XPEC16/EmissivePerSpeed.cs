using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmissivePerSpeed : MonoBehaviour
{
    public Color startColor;
    public Color endColor;
    public float maxSpeed = 5.0f;
    public string colorIndex = "_EmissionColor";

    private Rigidbody rb;
    private Renderer rend;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        rend.material.SetColor(colorIndex, Color.Lerp(startColor, endColor, rb.velocity.magnitude/maxSpeed));
    }
}
