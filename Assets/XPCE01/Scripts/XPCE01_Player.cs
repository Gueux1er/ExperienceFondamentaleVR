using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPCE01_Player : MonoBehaviour
{
    public bool isOnWater;
    public SineWaveControler sineWaveControler;

    private void Update()
    {
        if (XPCE01_WaterController.instance.waterObject.transform.position.y > transform.position.y)
        {
            isOnWater = true;
            sineWaveControler.Activate();
        }
        else
        {
            isOnWater = false;
            sineWaveControler.Desactivate();
        }
    }
}
