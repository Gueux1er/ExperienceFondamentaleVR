using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPCE_FollowThePlayer_BallScript : MonoBehaviour
{

    Color colorValue;
    Color BaseColor;
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
            
            colorValue -= new Color(0.005f, 0.005f, 0.005f);
            this.GetComponent<Renderer>().material.color = colorValue;
            if (colorValue.r <= 0 && colorValue.g <= 0 && colorValue.b <= 0)
            {
                Destroy(this.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name.Contains("Destruction"))
        {
            colorValue = BaseColor;
        }
    }
}
