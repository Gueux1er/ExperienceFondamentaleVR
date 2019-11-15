using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AkilliMum.Standard.Mirror;

public class XPCE19_MirrorManager : MonoBehaviour
{
    public CameraShade shade;
    
    [Header("Control")]
    [Range(0, 1)]
    public float intensity = 0;
    
    private void Update()
    {
        shade.Intensity = intensity;
    }
}
