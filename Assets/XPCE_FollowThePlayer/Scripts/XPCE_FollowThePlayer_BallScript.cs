using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPCE_FollowThePlayer_BallScript : MonoBehaviour
{

    Color colorValue;
    Color BaseColor;
    float smoothness = 0.02f;
    bool isCollided = false;

    // Start is called before the first frame update
    void Start()
    {
        colorValue = GetComponent<Renderer>().material.color;
        BaseColor = GetComponent<Renderer>().material.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.name.Contains("Destruction"))
        {
            //print("Contact");
            isCollided = true;

            colorValue -= new Color(0.005f, 0.005f, 0.005f);
            this.GetComponent<Renderer>().material.color = colorValue;
            if (colorValue.r <= 0 && colorValue.g <= 0 && colorValue.b <= 0)
            {
                Destroy(this.gameObject);
            }
            //print("contact end");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name.Contains("Destruction"))
        {
            isCollided = false;
            StartCoroutine(ComeBackToNormalColor());
        }
    }

    IEnumerator ComeBackToNormalColor()
    {
        float incrementation = 0;
        for (; ; )
        {
            colorValue = Color.Lerp(colorValue, BaseColor, incrementation);
            incrementation += smoothness;
            this.GetComponent<Renderer>().material.color = colorValue;
            if (isCollided == true)
            {
                yield break;
            }
            yield return null;
        }
    }
}
