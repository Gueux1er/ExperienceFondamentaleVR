using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPCE08_Scalling : MonoBehaviour
{
    float scaling;
    float speedOfScale;
    float scale = 0.1f;
    public float scaleMaximal;

    float pointInTime;


    // Start is called before the first frame update
    void Start()
    {
        print("restart");
        this.transform.GetComponent<Renderer>().material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

        speedOfScale = Random.Range(1, 10);

        scaleMaximal = Random.Range(0.1f, scaleMaximal);

        scale = this.transform.localScale.x;
        StartCoroutine(ScallingDuringTime());
    }

    // Update is called once per frame
    void Update()
    {
    }

    IEnumerator ScallingDuringTime()
    {
        for (; ; )
        {
            float movingScaling;
            movingScaling =  Mathf.PingPong(Time.time/speedOfScale,scaleMaximal) + scale;
            //print(movingScaling + " " + scale);
            this.transform.localScale = new Vector3(movingScaling, movingScaling, movingScaling);

            yield return null;
        }



    }
}
