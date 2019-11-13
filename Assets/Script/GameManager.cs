using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject env;

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.Two)) //A   reduce speed
        {
            GrapinInstance[] c = env.GetComponentsInChildren<GrapinInstance>();
            foreach(GrapinInstance cube in c)
            {
                cube.speed += 0.5f;
            }
        }

        if (OVRInput.GetDown(OVRInput.Button.Two)) //B   increase speed
        {
            GrapinInstance[] c = env.GetComponentsInChildren<GrapinInstance>();
            foreach (GrapinInstance cube in c)
            {
                cube.speed -= 0.5f;
            }
        }
    }
}
