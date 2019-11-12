using System.Collections;
using System.Collections.Generic;
using UnityEngine;  

public class SineWaveControler : MonoBehaviour
{
    private SineWave sineWave;

    private bool active = false;
    public float speed = 5.0f;
    public float amplitude = 1.0f;
    public float frequency = 1.0f;

    private void Start()
    {
        sineWave = GetComponent<SineWave>();
        Activate();
    }

    public void Activate()
    {
        active = true;
        sineWave.Amplitude = amplitude;
        sineWave.Frequency = frequency;
    }

    private void Update()
    {
        if (active)
            sineWave.VerticalOffset += Time.deltaTime * speed;
        //sineWave.Amplitude = amplitude;
        //sineWave.Frequency = frequency;
    }


    private float Oscillator()
    {
        return Mathf.Sin(Time.time * speed);
    }
}
